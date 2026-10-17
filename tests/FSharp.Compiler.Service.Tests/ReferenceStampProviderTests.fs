// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// The file system shim and the incremental builder's event log are global shared resources.
[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharp.Compiler.Service.Tests.ReferenceStampProviderTests

open System
open System.Collections.Concurrent
open System.IO
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.IO
open FSharp.Compiler.Service.Tests.Common
open Xunit

/// `Main.fs` referencing a `Lib.dll` compiled into the same temp directory: a non-framework reference whose
/// stamp the incremental builder has to track between requests.
type private Fixture =
    {
        LibDll: string
        LibFs: string
        LibOptions: FSharpProjectOptions
        MainFs: string
        Options: FSharpProjectOptions
    }

let private withoutDocFile (args: string[]) =
    args |> Array.filter (fun arg -> not (arg.StartsWith("--doc:", StringComparison.Ordinal)))

let private withFixture (checker: FSharpChecker) (test: Fixture -> Task) : Task =
    task {
        let dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
        Directory.CreateDirectory dir |> ignore

        try
            let libFs = Path.Combine(dir, "Lib.fs")
            File.WriteAllText(libFs, "module Lib\nlet answer = 42\n")
            let libDll = Path.Combine(dir, "Lib.dll")
            let libArgs = mkProjectCommandLineArgsSilent (libDll, [| libFs |]) |> withoutDocFile

            let! diagnostics, exn = checker.Compile(Array.append [| "fsc.exe" |] libArgs)
            Assert.True(exn.IsNone, $"compiling Lib.dll threw: %A{exn}")
            Assert.Empty(diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))

            let mainFs = Path.Combine(dir, "Main.fs")
            File.WriteAllText(mainFs, "module Main\nlet x = Lib.answer + 1\n")

            let mainArgs =
                [|
                    yield! mkProjectCommandLineArgsSilent (Path.Combine(dir, "Main.dll"), [| mainFs |])
                    "-r:" + libDll
                |]

            do!
                test
                    {
                        LibDll = libDll
                        LibFs = libFs
                        LibOptions =
                            { checker.GetProjectOptionsFromCommandLineArgs(Path.Combine(dir, "Lib.fsproj"), libArgs) with
                                SourceFiles = [| libFs |]
                            }
                        MainFs = mainFs
                        Options =
                            { checker.GetProjectOptionsFromCommandLineArgs(Path.Combine(dir, "Main.fsproj"), mainArgs) with
                                SourceFiles = [| mainFs |]
                            }
                    }
        finally
            try
                Directory.Delete(dir, true)
            with _ ->
                ()
    }

let private recording () =
    let seen = ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    seen, (fun (path: string) -> seen.AddOrUpdate(path, 1, (fun _ count -> count + 1)) |> ignore)

[<Fact>]
let ``The builder asks the provider for reference stamps instead of the file system`` () : Task =
    let checker = FSharpChecker.Create(useTransparentCompiler = false)

    withFixture checker (fun fixture ->
        task {
            let statted, recordStat = recording ()
            let asked, recordAsk = recording ()
            let originalFileSystem = FileSystem

            FileSystem <-
                { new DefaultFileSystem() with
                    override _.GetLastWriteTimeShim path =
                        recordStat path
                        File.GetLastWriteTimeUtc path
                }

            try
                checker.SetReferenceStampProvider(fun path ->
                    recordAsk path
                    File.GetLastWriteTimeUtc path)

                // Creating the builder reads the dll to import it; every request after that is what matters.
                let! _ = checker.ParseAndCheckProject fixture.Options
                statted.Clear()
                asked.Clear()

                let! _ = checker.ParseAndCheckProject fixture.Options

                Assert.True(asked.ContainsKey fixture.LibDll, "the provider was not asked for the reference's stamp")
                Assert.False(statted.ContainsKey fixture.LibDll, "the reference was stat'd although the host answers its stamp")
            finally
                FileSystem <- originalFileSystem
        })

[<Fact>]
let ``A newer host stamp recreates the builder and a newer file alone does not`` () : Task =
    let checker = FSharpChecker.Create(useTransparentCompiler = false)

    withFixture checker (fun fixture ->
        task {
            let stamps = ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase)
            checker.SetReferenceStampProvider(fun path -> stamps.GetOrAdd(path, (fun path -> File.GetLastWriteTimeUtc path)))

            let mark = IncrementalBuilderEventTesting.GetCurrentIncrementalBuildEventNum()

            let buildersCreated () =
                IncrementalBuilderEventTesting.GetMostRecentIncrementalBuildEvents(
                    IncrementalBuilderEventTesting.GetCurrentIncrementalBuildEventNum() - mark
                )
                |> List.filter ((=) IncrementalBuilderEventTesting.IBECreated)
                |> List.length

            let! _ = checker.ParseAndCheckProject fixture.Options
            Assert.Equal(1, buildersCreated ())

            // The file is newer, but the host has not said so.
            File.SetLastWriteTimeUtc(fixture.LibDll, File.GetLastWriteTimeUtc fixture.LibDll + TimeSpan.FromSeconds 2.)
            let! _ = checker.ParseAndCheckProject fixture.Options
            Assert.Equal(1, buildersCreated ())

            stamps[fixture.LibDll] <- File.GetLastWriteTimeUtc fixture.LibDll
            let! _ = checker.ParseAndCheckProject fixture.Options
            Assert.Equal(2, buildersCreated ())

            // The recreated builder stamped through the same source, so nothing is stale any more.
            let! _ = checker.ParseAndCheckProject fixture.Options
            Assert.Equal(2, buildersCreated ())
        })

[<Fact>]
let ``Project references keep their logical stamps`` () : Task =
    let checker = FSharpChecker.Create(useTransparentCompiler = false)

    withFixture checker (fun fixture ->
        task {
            // Lib is also an in-memory project reference; the provider knows nothing about any file.
            let options =
                { fixture.Options with
                    ReferencedProjects = [| FSharpReferencedProject.FSharpReference(fixture.LibDll, fixture.LibOptions) |]
                }

            checker.SetReferenceStampProvider(fun _ -> DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))

            let mainChecks = ref 0

            checker.FileChecked.Add(fun (fileName, _) ->
                if String.Equals(fileName, fixture.MainFs, StringComparison.OrdinalIgnoreCase) then
                    Interlocked.Increment &mainChecks.contents |> ignore)

            // The second request lets Main's builder see Lib's build, which did not exist when Main was first stamped.
            let! _ = checker.ParseAndCheckProject options
            let! _ = checker.ParseAndCheckProject options
            let before = mainChecks.Value

            // Lib's source moves its logical stamp, which Main reads from the referenced build, not from the provider.
            File.WriteAllText(fixture.LibFs, "module Lib\nlet answer = 43\n")
            File.SetLastWriteTimeUtc(fixture.LibFs, DateTime.UtcNow.AddSeconds 2.)
            let! _ = checker.ParseAndCheckProject options

            Assert.Equal(before + 1, mainChecks.Value)
        })

#nowarn "57" // NotifyFileChanged is experimental

[<Fact>]
let ``The transparent compiler hands the provider to the builders it still creates`` () : Task =
    let checker = FSharpChecker.Create(useTransparentCompiler = true)

    withFixture checker (fun fixture ->
        task {
            let asked, recordAsk = recording ()

            checker.SetReferenceStampProvider(fun path ->
                recordAsk path
                File.GetLastWriteTimeUtc path)

            // NotifyFileChanged is the one request the transparent compiler still routes to a legacy builder.
            do! checker.NotifyFileChanged(fixture.MainFs, fixture.Options)

            Assert.True(asked.ContainsKey fixture.LibDll)
        })

#warnon "57"
