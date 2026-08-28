// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.PortableExecutable

module determinism =

    let areSame first second =
        let load = System.IO.File.ReadAllBytes
        if not ((load first) = (load second)) then
            raise (new Exception "Pathmap1 and PathMap2 do not match")

    let compileSource options compilation =
        compilation
        |> asLibrary
        |> withOptionsString options
        |> compile

    [<InlineData("--deterministic")>]
    [<InlineData("--deterministic+")>]
    [<InlineData("--deterministic-")>]
    [<InlineData("--deterministic;--debug:full")>]
    [<InlineData("--deterministic;--debug:pdbonly")>]
    [<InlineData("--deterministic;--debug:portable")>]
    [<InlineData("--deterministic;--debug:embedded")>]
    [<InlineData("--deterministic+;--debug:embedded")>]
    [<InlineData("--deterministic-;--debug:embedded")>]
    [<Theory>]
    let ``smoketest options`` options =
        FSharp """
module Determinism
"""
        |> compileSource options
        |> shouldSucceed

    [<InlineData("--deterministic")>]
    [<InlineData("--deterministic-")>]
    [<InlineData("--deterministic+")>]
    [<Theory>]
    let ``Confirm specific version allowed`` options =
        FSharp """
module Determinism
[<assembly: System.Reflection.AssemblyVersion("2.3.4.5")>]
do()
"""
        |> compileSource options
        |> shouldSucceed

    [<InlineData("--deterministic-")>]
    [<Theory>]
    let ``Confirm wildcard version allowed`` options =
        FSharp """
module Determinism
[<assembly: System.Reflection.AssemblyVersion("2.3.4.*")>]
do ()
"""
        |> compileSource options
        |> shouldSucceed

    [<InlineData("--deterministic+")>]
    [<Theory>]
    let ``Confirm wildcard version not allowed`` options =
        FSharp """
module Determinism
[<assembly: System.Reflection.AssemblyVersion("2.3.4.*")>]
do ()
"""
        |> compileSource options
        |> shouldFail
        |> withDiagnostics [
            (Error 2025, Line 1, Col 1, Line 1, Col 1, "An AssemblyVersionAttribute specified version '2.3.4.*', but this value is a wildcard, and you have requested a deterministic build, these are in conflict.")
            ]

    [<Fact>]
    let ``Invalid pathmap value`` () =
        FSharp """
module Determinism
"""
        |> compileSource @"--pathmap:C:\NoOtherPath;--debug:embedded"
        |> shouldFail
        |> withDiagnostics [
            (Error 2028, Line 0, Col 1, Line 0, Col 1, "Invalid path map. Mappings must be comma separated and of the format 'path=sourcePath'")
            ]

    [<Fact>]
    let ``pathmap with Embedded Pdbs`` () =
        let thisTestDirectory = getTestOutputDirectory __SOURCE_DIRECTORY__ (getCurrentMethodName()) ""
        let pathMap1 =
            let compilation =
                FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  @"PathMap1/pathmap.fs"))
                |> withOutputDirectory thisTestDirectory
            compilation
            |> withOptionsString $"""--pathmap:{compilation.OutputDirectory}/PathMap1=/src,F:\=/etc;--deterministic;--embed;--debug:embedded"""
            |> asExe
            |> compile

        let pathMap2 =
            let compilation =
                FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  @"PathMap2/pathmap.fs"))
                |> withOutputDirectory thisTestDirectory
            compilation
            |> withOptionsString $"""--pathmap:{compilation.OutputDirectory}/PathMap2=/src,F:\=/etc;--deterministic;--embed;--debug:embedded"""
            |> asExe
            |> compile

        match pathMap1.Output.OutputPath, pathMap2.Output.OutputPath with
        | Some exename1, Some exename2 ->
            areSame exename1 exename2
        | _ -> raise (new Exception "Pathmap1 and PathMap2 do not match")

    [<Fact>]
    let ``pathmap with Portable Pdbs`` () =
        let thisTestDirectory = getTestOutputDirectory __SOURCE_DIRECTORY__ (getCurrentMethodName()) ""
        let pathMap1 =
            let compilation =
                FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  @"PathMap1/pathmap.fs"))
                |> withOutputDirectory thisTestDirectory
            compilation
            |> withOptionsString $"""--pathmap:{compilation.OutputDirectory}/PathMap1=/src,F:\=/etc;--deterministic;--embed;--debug:portable"""
            |> asExe
            |> compile

        let pathMap2 =
            let compilation =
                FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  @"PathMap2/pathmap.fs"))
                |> withOutputDirectory thisTestDirectory
            compilation
            |> withOptionsString $"""--pathmap:{compilation.OutputDirectory}/PathMap2=/src,F:\=/etc;--deterministic;--embed;--debug:portable"""
            |> asExe
            |> compile

        match pathMap1.Output.OutputPath, pathMap2.Output.OutputPath with
        | Some exename1, Some exename2 ->
            areSame exename1 exename2
            areSame (Path.ChangeExtension(exename1, "pdb")) (Path.ChangeExtension(exename2, "pdb"))
        | _ -> raise (new Exception "Pathmap1 and PathMap2 do not match")

    /// Compile to ref assembly out-of-process via runFscProcess.
    /// Separate processes needed because String.GetHashCode is seeded once per process.
    let private compileRefAssembly (workDir: string) (sourceFile: string) : string * string =
        Directory.CreateDirectory workDir |> ignore
        let outDll = Path.Combine(workDir, "Out.dll")
        let outRef = Path.Combine(workDir, "Out.ref.dll")
        let defaultOpts = CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions
        let result = runFscProcess [
            yield "--target:library"
            yield "--deterministic+"
            yield! (defaultOpts |> Array.toList)
            yield $"--refout:{outRef}"
            yield $"-o:{outDll}"
            yield sourceFile
        ]
        if result.ExitCode <> 0 then
            failwithf "fsc exit %d\nstdout:%s\nstderr:%s" result.ExitCode result.StdOut result.StdErr
        outDll, outRef

    let private readMvid (dll: string) : Guid =
        use peReader = new PEReader(File.OpenRead dll)
        let reader = peReader.GetMetadataReader()
        reader.GetGuid(reader.GetModuleDefinition().Mvid)

    /// As compileRefAssembly, but for a multi-file compilation.
    let private compileRefAssemblyOfFiles (workDir: string) (sourceFiles: string list) : string =
        Directory.CreateDirectory workDir |> ignore
        let outDll = Path.Combine(workDir, "Out.dll")
        let outRef = Path.Combine(workDir, "Out.ref.dll")
        let defaultOpts = CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions
        let result = runFscProcess [
            yield "--target:library"
            yield "--deterministic+"
            // As a Debug build compiles. With optimizations on, the embedded optimization
            // data changes when a member is renamed and `optDataHash` - a SHA over those
            // resources, and not truncated - carries that into the MVID, masking the
            // truncated per-file signature hash below.
            yield "--optimize-"
            yield! (defaultOpts |> Array.toList)
            yield $"--refout:{outRef}"
            yield $"-o:{outDll}"
            yield! sourceFiles
        ]
        if result.ExitCode <> 0 then
            failwithf "fsc exit %d\nstdout:%s\nstderr:%s" result.ExitCode result.StdOut result.StdErr
        outRef

    /// `fileCount` single-module files, where the public member in file `renameAt` (1-based)
    /// is called `memberName` instead of the default. Everything else is identical.
    let private writeModules (dir: string) (fileCount: int) (renameAt: int) (memberName: string) =
        Directory.CreateDirectory dir |> ignore
        [ for i in 1 .. fileCount ->
            let path = Path.Combine(dir, sprintf "File%02d.fs" i)
            let name = if i = renameAt then memberName else sprintf "value%02d" i
            File.WriteAllText(path, sprintf "module M%02d\n\nlet %s (x: int) : int = x + %d\n" i name i)
            path ]

    /// Renaming a public member changes the assembly's public API, so it has to change the
    /// reference assembly's MVID - otherwise MSBuild's CopyRefAssembly sees an unchanged MVID,
    /// skips the copy, and every downstream project keeps compiling against the old surface.
    ///
    /// It does not, for a Debug (`--optimize-`) compilation, if the file is far enough from
    /// the end of the compilation order.
    /// `calculateSignatureHashOfFiles` folds the per-file signature hashes with
    /// `combineHash acc y = (acc <<< 1) + y + 631` over `type Hash = int` (TypeHashing.fs), so
    /// unrolled the accumulator is `sum over i of (y_i + 631) * pown 2 (N - i)`. In 32-bit
    /// arithmetic file i contributes exactly zero once `N - i >= 32`.
    ///
    /// Latent since #15325; only observable since #19751 made the hash deterministic across
    /// processes - before that every compile produced a fresh MVID, so the copy always happened.
    [<FactForNETCOREAPP>]
    let ``Reference assembly MVID changes when a public member is renamed in an early file`` () =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "fsharp-ref-mvid-position-" + Guid.NewGuid().ToString("N"))
        try
            let fileCount = 40

            let before = writeModules (Path.Combine(tempRoot, "before")) fileCount 1 "value01"
            let after = writeModules (Path.Combine(tempRoot, "after")) fileCount 1 "renamedValue01"

            let refBefore = compileRefAssemblyOfFiles (Path.Combine(tempRoot, "outBefore")) before
            let refAfter = compileRefAssemblyOfFiles (Path.Combine(tempRoot, "outAfter")) after

            Assert.NotEqual(readMvid refBefore, readMvid refAfter)
        finally
            try Directory.Delete(tempRoot, true) with _ -> ()

    /// Control for the test above: the identical rename in the LAST file is picked up. This is
    /// what makes the failure positional rather than "renames are ignored" - and it fails too
    /// if the harness itself stops discriminating.
    [<FactForNETCOREAPP>]
    let ``Reference assembly MVID changes when a public member is renamed in the last file`` () =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "fsharp-ref-mvid-position-" + Guid.NewGuid().ToString("N"))
        try
            let fileCount = 40

            let before = writeModules (Path.Combine(tempRoot, "before")) fileCount fileCount "value40"
            let after = writeModules (Path.Combine(tempRoot, "after")) fileCount fileCount "renamedValue40"

            let refBefore = compileRefAssemblyOfFiles (Path.Combine(tempRoot, "outBefore")) before
            let refAfter = compileRefAssemblyOfFiles (Path.Combine(tempRoot, "outAfter")) after

            Assert.NotEqual(readMvid refBefore, readMvid refAfter)
        finally
            try Directory.Delete(tempRoot, true) with _ -> ()

    // Regression test for https://github.com/dotnet/fsharp/issues/19751
    // Two separate fsc processes needed to detect randomized String.GetHashCode seeds.
    [<FactForNETCOREAPP>]
    let ``Reference assembly MVID is deterministic across separate fsc invocations`` () =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "fsharp-ref-mvid-test-" + Guid.NewGuid().ToString("N"))
        try
            Directory.CreateDirectory tempRoot |> ignore
            let src = Path.Combine(tempRoot, "Foo.fs")
            File.WriteAllText(src, "module Foo.Core\n\nlet foo (x: int) : int = x + 1\n")

            let dll1, ref1 = compileRefAssembly (Path.Combine(tempRoot, "out1")) src
            let dll2, ref2 = compileRefAssembly (Path.Combine(tempRoot, "out2")) src

            Assert.Equal(readMvid ref1, readMvid ref2)
            Assert.Equal(readMvid dll1, readMvid dll2)
        finally
            try Directory.Delete(tempRoot, true) with _ -> ()
