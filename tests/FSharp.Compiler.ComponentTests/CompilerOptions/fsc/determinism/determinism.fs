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
