// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit

/// Minimal ITaskHost stand-in matching the shape MSBuild passes to Fsc/Fsi via HostObject. Its
/// Compile method is discovered by reflection in Fsc/Fsi.ExecuteTool.
type FauxHostObject() =
    let mutable flags: string[] = [||]
    let mutable sources: string[] = [||]

    member _.Compile(_compile: Func<int>, flagsIn: string[], sourcesIn: string[]) =
        flags <- flagsIn
        sources <- sourcesIn
        0

    member _.Flags = flags
    member _.Sources = sources

    interface ITaskHost

/// Reflection helpers for the internal Fsc/Fsi test hooks (InternalsVisibleTo is granted only to
/// VisualFSharp.UnitTests, so this assembly reaches them by reflection).
module private FscFsiTestHooks =

    let private invoke (task: obj) (name: string) (args: obj[]) =
        let m =
            FSharp.Test.ReflectionHelper.getPrivateInstanceMethod name (task.GetType())

        m.Invoke(task, args)

    let fullPathToTool (task: obj) : string =
        invoke task "InternalGenerateFullPathToTool" [||] :?> string

    let generateResponseFileCommands (task: obj) : string =
        invoke task "InternalGenerateResponseFileCommands" [||] :?> string

    let executeTool (task: obj) : int =
        invoke task "InternalExecuteTool" [| box ""; box ""; box "" |] :?> int

    let normalizePathToTool (task: obj) (pathToTool: string) : string =
        invoke task "InternalNormalizePathToTool" [| box pathToTool |] :?> string

/// Verifies the multithread-safe FSharp.Build tasks are marked directly with
/// MSBuildMultiThreadableTaskAttribute, opting them into MSBuild's multi-threaded scheduler.
type MultiThreadedTaskTests() =

    static member MultiThreadableTaskTypes: obj[] seq =
        seq {
            yield [| typeof<CreateFSharpManifestResourceName> |]
            yield [| typeof<MapSourceRoots> |]
            yield [| typeof<WriteCodeFragment> |]
            yield [| typeof<GenerateILLinkSubstitutions> |]
            yield [| typeof<FSharpEmbedResourceText> |]
            yield [| typeof<FSharpEmbedResXSource> |]
            yield [| typeof<SubstituteText> |]
            yield [| typeof<Fsc> |]
            yield [| typeof<Fsi> |]
        }

    [<Theory>]
    [<MemberData(nameof MultiThreadedTaskTests.MultiThreadableTaskTypes)>]
    member _.``task is marked directly multithreadable`` (taskType: Type) =
        let attributes = taskType.GetCustomAttributes(typeof<MSBuildMultiThreadableTaskAttribute>, false)

        Assert.Equal(1, attributes.Length)

/// Verifies Fsc and Fsi resolve their tool location and capture per-invocation state against the
/// TaskEnvironment injected into their constructor, so concurrent instances stay isolated.
type FscFsiMultiThreadedTaskTests() =

    /// Creates a TaskEnvironment whose FSHARP_COMPILER_BIN points at a fresh temporary directory,
    /// returned with that directory. Callers must dispose the environment.
    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        let compilerBin = TestFramework.createTemporaryDirectory().FullName

        let variables = Dictionary<string, string>()
        variables["FSHARP_COMPILER_BIN"] <- compilerBin

        let environment =
            TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables)

        environment, compilerBin

    /// As withCompilerBinEnvironment but with two independent environments, disposing both afterwards.
    static let withCompilerBinEnvironmentPair body =
        let environmentA, binA = environmentWithCompilerBin ()

        try
            let environmentB, binB = environmentWithCompilerBin ()

            try
                body environmentA binA environmentB binB
            finally
                BuildTaskTestHelpers.disposeTaskEnvironment environmentB
        finally
            BuildTaskTestHelpers.disposeTaskEnvironment environmentA

    [<Fact>]
    member _.``Fsc and Fsi resolve tool path against distinct injected compiler-bin environments``() =
        withCompilerBinEnvironmentPair (fun fscEnvironment fscBin fsiEnvironment fsiBin ->
            let fsc = Fsc(fscEnvironment)
            let fsi = Fsi(fsiEnvironment)

            let fscPath = FscFsiTestHooks.fullPathToTool fsc
            let fsiPath = FscFsiTestHooks.fullPathToTool fsi

            Assert.True(Path.IsPathRooted fscPath, $"expected rooted path, got '{fscPath}'")
            Assert.True(Path.IsPathRooted fsiPath, $"expected rooted path, got '{fsiPath}'")

            Assert.Equal("fsc.exe", Path.GetFileName fscPath)
            Assert.Equal("fsi.exe", Path.GetFileName fsiPath)

            Assert.Equal(Path.GetFullPath fscBin, Path.GetDirectoryName fscPath)
            Assert.Equal(Path.GetFullPath fsiBin, Path.GetDirectoryName fsiPath)

            Assert.NotEqual<string>(Path.GetDirectoryName fscPath, Path.GetDirectoryName fsiPath))

    [<Fact>]
    member _.``Two Fsc tasks resolve tool paths against their own injected compiler-bin environments``() =
        withCompilerBinEnvironmentPair (fun firstEnvironment firstBin secondEnvironment secondBin ->
            let first = Fsc(firstEnvironment)
            let second = Fsc(secondEnvironment)

            let firstPath = FscFsiTestHooks.fullPathToTool first
            let secondPath = FscFsiTestHooks.fullPathToTool second

            Assert.Equal(Path.GetFullPath firstBin, Path.GetDirectoryName firstPath)
            Assert.Equal(Path.GetFullPath secondBin, Path.GetDirectoryName secondPath)
            Assert.NotEqual<string>(firstPath, secondPath))

    [<Fact>]
    member _.``Two concurrent Fsc tasks route flags and sources to their own host objects``() =
        let makeTask (flag: string) (sourceNames: string list) =
            let task = Fsc()
            task.BuildEngine <- MockEngine()
            task.OtherFlags <- flag

            task.Sources <-
                sourceNames
                |> List.map (fun name -> TaskItem(name) :> ITaskItem)
                |> List.toArray

            let host = FauxHostObject()
            task.HostObject <- host
            task, host

        let firstTask, firstHost = makeTask "--firstflag" [ "first1.fs"; "first2.fs" ]
        let secondTask, secondHost = makeTask "--secondflag" [ "second1.fs" ]

        // A barrier maximises the chance both tasks are inside their compile path simultaneously.
        use barrier = new Barrier(2)

        let run (task: Fsc) =
            Task.Run(fun () ->
                FscFsiTestHooks.generateResponseFileCommands task |> ignore
                barrier.SignalAndWait()
                FscFsiTestHooks.executeTool task |> ignore)

        Task.WaitAll(run firstTask, run secondTask)

        Assert.Equal<string[]>([| "first1.fs"; "first2.fs" |], firstHost.Sources)
        Assert.Equal<string[]>([| "second1.fs" |], secondHost.Sources)

        Assert.Contains("--firstflag", firstHost.Flags)
        Assert.DoesNotContain("--secondflag", firstHost.Flags)

        Assert.Contains("--secondflag", secondHost.Flags)
        Assert.DoesNotContain("--firstflag", secondHost.Flags)

    [<Fact>]
    member _.``Fsc roots a relative pathToTool against its injected project directory``() =
        BuildTaskTestHelpers.withTaskEnvironment (fun environment directory ->
            let fsc = Fsc(environment)
            let relative = Path.Combine("tools", "fsc.exe")

            let normalized = FscFsiTestHooks.normalizePathToTool fsc relative

            Assert.True(Path.IsPathRooted normalized, $"expected rooted path, got '{normalized}'")
            Assert.Equal(Path.Combine(directory.FullName, relative), normalized)

            // A bare filename must be left untouched so the OS/ComputePathToTool PATH lookup still works.
            Assert.Equal("fsc.exe", FscFsiTestHooks.normalizePathToTool fsc "fsc.exe")

            // Windows root-relative and drive-relative forms, which IsPathRooted reports as rooted yet
            // still bind to ambient state, must be routed through GetAbsolutePath (Windows-only shapes).
            if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                let rootRelative = @"\tools\fsc.exe"

                Assert.Equal(
                    environment.GetAbsolutePath(rootRelative).Value,
                    FscFsiTestHooks.normalizePathToTool fsc rootRelative
                )

                let driveRelative = @"C:tools\fsc.exe"

                Assert.Equal(
                    environment.GetAbsolutePath(driveRelative).Value,
                    FscFsiTestHooks.normalizePathToTool fsc driveRelative
                ))

    [<Fact>]
    member _.``Fsi roots a relative pathToTool against its injected project directory``() =
        BuildTaskTestHelpers.withTaskEnvironment (fun environment directory ->
            let fsi = Fsi(environment)
            let relative = Path.Combine("tools", "fsi.exe")

            let normalized = FscFsiTestHooks.normalizePathToTool fsi relative

            Assert.True(Path.IsPathRooted normalized, $"expected rooted path, got '{normalized}'")
            Assert.Equal(Path.Combine(directory.FullName, relative), normalized)

            // A bare filename must be left untouched so the OS/ComputePathToTool PATH lookup still works.
            Assert.Equal("fsi.exe", FscFsiTestHooks.normalizePathToTool fsi "fsi.exe")

            // Windows root-relative and drive-relative forms must also be rooted through GetAbsolutePath.
            if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                let rootRelative = @"\tools\fsi.exe"

                Assert.Equal(
                    environment.GetAbsolutePath(rootRelative).Value,
                    FscFsiTestHooks.normalizePathToTool fsi rootRelative
                )

                let driveRelative = @"C:tools\fsi.exe"

                Assert.Equal(
                    environment.GetAbsolutePath(driveRelative).Value,
                    FscFsiTestHooks.normalizePathToTool fsi driveRelative
                ))
