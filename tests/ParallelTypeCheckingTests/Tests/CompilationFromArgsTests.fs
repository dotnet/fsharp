module ParallelTypeCheckingTests.CompilationFromArgsTests

open NUnit.Framework
open System
open FSharp.Compiler
open FSharp.Compiler.Service.Tests
open NUnit.Framework
open Utils

let codebases =
    [
        @"$CODE_ROOT$\tests\FSharp.Compiler.ComponentTests", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\ComponentTests_args.txt"
        @"$CODE_ROOT$\src\compiler", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\Tests\FCSArgs.txt"
    ]

let configs =
    
        
    methods
    |> List.allPairs codebases
    |> List.map (fun ((workDir, path), method) -> 
        {
            Path = path
            WorkingDir = Some workDir
            Mode = method 
        }
    )

let setupArgsMethod (method: Method) (args: string[]): string[] =
    printfn $"Method: {method}"
    match method with
        | Method.Sequential ->
            // Restore default
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParseAndCheckInputs.CheckMultipleInputsInParallel
            args
        | Method.ParallelFs ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParseAndCheckInputs.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Graph ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Real.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Nojaf ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Nojaf.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]

let setupParsed config =
    let {Path = path; Mode = mode; WorkingDir = workingDir} = config
    let args =
        System.IO.File.ReadAllLines(path |> replacePaths)
        |> Array.map replacePaths
        
    printfn $"WorkingDir = {workingDir}"
    printfn $"%+A{args}"
    let args = setupArgsMethod mode args
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- replaceCodeRoot dir)
    args

[<TestCaseSource(nameof(configs))>]
let TestCompilerFromArgs (config : Args) : unit =
    let oldWorkDir = Environment.CurrentDirectory
    try
        let args = setupParsed config        
        let exit = CommandLineMain.main args
        Assert.That(exit, Is.Zero)
    finally
        Environment.CurrentDirectory <- oldWorkDir
