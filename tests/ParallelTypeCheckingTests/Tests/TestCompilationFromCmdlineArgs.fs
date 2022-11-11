module ParallelTypeCheckingTests.TestCompilationFromCmdlineArgs

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.DiagnosticsLogger
open NUnit.Framework
open System
open FSharp.Compiler
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.TestUtils

type Codebase =
    {
        WorkDir: string
        Path: string
        Limit: int option
    }

let codebases =
    [|
        {
            WorkDir = $@"{__SOURCE_DIRECTORY__}\.fcs_test\src\compiler"
            Path = $@"{__SOURCE_DIRECTORY__}\FCS.args.txt"
            Limit = None
        }
        {
            WorkDir = $@"{__SOURCE_DIRECTORY__}\.fcs_test\tests\FSharp.Compiler.ComponentTests"
            Path = $@"{__SOURCE_DIRECTORY__}\ComponentTests.args.txt"
            Limit = None
        }
    |]

let internal setupParsed config =
    let {
            Path = path
            LineLimit = lineLimit
            Method = method
            WorkingDir = workingDir
        } =
        config

    let args =
        System.IO.File.ReadAllLines(path |> replacePaths)
        |> fun lines ->
            match lineLimit with
            | Some limit -> Array.take (Math.Min(limit, lines.Length)) lines
            | None -> lines
        |> Array.map replacePaths

    setupCompilationMethod method

    printfn $"Method: {method}"
    let args =
        match method with
        | Method.Sequential -> args
        | Method.ParallelCheckingOfBackedImplFiles ->
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Graph ->
            Array.append args [|"--test:GraphBasedChecking"|] 
    
    printfn $"WorkingDir = {workingDir}"
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- dir)
    args

let internal TestCompilerFromArgs (config: Args) : unit =
    use _ =
        FSharp.Compiler.Diagnostics.Activity.start "Compile codebase" [ "method", config.Method.ToString() ]

    let oldWorkDir = Environment.CurrentDirectory

    let exiter =
        { new Exiter with
            member _.Exit n =
                Assert.Fail($"Fail - {n} errors found")
                failwith ""
        }

    try
        let args = setupParsed config
        let exit: int = CommandLineMain.mainAux (args, true, Some exiter)
        Assert.That(exit, Is.Zero)
    finally
        Environment.CurrentDirectory <- oldWorkDir

let internal codebaseToConfig code method =
    {
        Path = code.Path
        LineLimit = code.Limit
        Method = method
        WorkingDir = Some code.WorkDir
    }

[<TestCaseSource(nameof (codebases))>]
[<Explicit("Before running these tests, you must prepare the codebase by running FCS.prepare.ps1")>]
let ``Test graph-based type-checking`` (code: Codebase) =
    let config = codebaseToConfig code Method.Graph
    TestCompilerFromArgs config

[<TestCaseSource(nameof (codebases))>]
[<Explicit("Before running these tests, you must prepare the codebase by running FCS.prepare.ps1")>]
let ``Test sequential type-checking`` (code: Codebase) =
    let config = codebaseToConfig code Method.Sequential
    TestCompilerFromArgs config
