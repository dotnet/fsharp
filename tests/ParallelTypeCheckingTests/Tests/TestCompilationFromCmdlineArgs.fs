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
        WorkDir : string
        Path : string
        Limit : int option
    }

let codebases =
    [|
        { WorkDir = $@"{__SOURCE_DIRECTORY__}\.checkouts\fcs\src\compiler"; Path = $@"{__SOURCE_DIRECTORY__}\FCS.args.txt"; Limit = None }
        { WorkDir = $@"{__SOURCE_DIRECTORY__}\.checkouts\fcs\tests\FSharp.Compiler.ComponentTests"; Path = $@"{__SOURCE_DIRECTORY__}\ComponentTests.args.txt"; Limit = None }
    |]

/// A very hacky way to setup the given type-checking method - mutates static state and returns new args
/// TODO Make the method configurable via proper config passed top-down
let internal setupArgsMethod (method: TypeCheckingMode) (args: string[]): string[] =
    printfn $"Method: {method}"
    match method with
        | TypeCheckingMode.Sequential ->
            // Restore default
            ParseAndCheckInputs.CheckMultipleInputsUsingGraphMode <- ParseAndCheckInputs.CheckMultipleInputsInParallel
            args
        | TypeCheckingMode.ParallelCheckingOfBackedImplFiles ->
            ParseAndCheckInputs.CheckMultipleInputsUsingGraphMode <- ParseAndCheckInputs.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | TypeCheckingMode.Graph ->
            ParseAndCheckInputs.CheckMultipleInputsUsingGraphMode <- ParallelTypeChecking.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]

let internal setupParsed config =
    let {Path = path; LineLimit = lineLimit; Method = method; WorkingDir = workingDir} = config
    let args =
        System.IO.File.ReadAllLines(path |> replacePaths)
        |> fun lines -> match lineLimit with Some limit -> Array.take (Math.Min(limit, lines.Length)) lines | None -> lines 
        |> Array.map replacePaths
        
    printfn $"WorkingDir = {workingDir}"
    let args = setupArgsMethod method args
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- replaceCodeRoot dir)
    args

let internal TestCompilerFromArgs (config : Args) : unit =
    use _ = FSharp.Compiler.Diagnostics.Activity.start "Compile codebase" ["method", config.Method.ToString()]
    let oldWorkDir = Environment.CurrentDirectory
    
    let exiter =
        { new Exiter with
                        member _.Exit n =
                            Assert.Fail($"Fail - {n} errors found")
                            failwith ""
                    }
        
    try
        let args = setupParsed config
        let exit : int = CommandLineMain.mainAux(args, true, Some exiter)
        Assert.That(exit, Is.Zero)
    finally
        Environment.CurrentDirectory <- oldWorkDir

[<TestCaseSource(nameof(codebases))>]
[<Explicit("Before running these tests, you must prepare the codebase by running FCS.prepare.ps1")>]
let ``Test graph-based type-checking`` (code : Codebase) =
    let config =
        {
            Path = code.Path
            LineLimit = code.Limit
            Method = TypeCheckingMode.Graph
            WorkingDir = Some code.WorkDir
        }
    TestCompilerFromArgs config