module ParallelTypeCheckingTests.TestCompilationFromCmdlineArgs

open FSharp.Compiler.DiagnosticsLogger
open NUnit.Framework
open System
open FSharp.Compiler
open ParallelTypeCheckingTests
open NUnit.Framework
open ParallelTypeCheckingTests.TestUtils
open Utils

let codebases =
    [
        @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", None
    ]

let configs =
    [Method.Graph]
    |> List.allPairs codebases
    |> List.map (fun ((workDir, path, lineLimit : int option), method) -> 
        {
            Path = path
            LineLimit = lineLimit
            WorkingDir = Some workDir
            Method = method 
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
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Nojaf ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- SingleTcStateTypeChecking.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]

let setupParsed config =
    let {Path = path; LineLimit = lineLimit; Method = method; WorkingDir = workingDir} = config
    let args =
        System.IO.File.ReadAllLines(path |> replacePaths)
        |> fun lines -> match lineLimit with Some limit -> Array.take (Math.Min(limit, lines.Length)) lines | None -> lines 
        |> Array.map replacePaths
        
    printfn $"WorkingDir = {workingDir}"
    let args = setupArgsMethod method args
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- replaceCodeRoot dir)
    args

[<TestCaseSource(nameof(configs))>]
let TestCompilerFromArgs (config : Args) : unit =
    use _ = FSharp.Compiler.Diagnostics.Activity.start "Compile codebase" ["method", config.Method.ToString()]
    let oldWorkDir = Environment.CurrentDirectory
    
    let exiter =
        { new Exiter with
                        member _.Exit n =
                            Assert.Fail($"Fail - {n} errors found")
                            failwith (FSComp.SR.elSysEnvExitDidntExit ())
                    }
        
    try
        let args = setupParsed config
        let args = args |> Array.filter (fun x -> not <| x.Contains("Activity.fs"))
        let exit : int = CommandLineMain.mainAux2(args, true, Some exiter)
        Assert.That(exit, Is.Zero)
    finally
        Environment.CurrentDirectory <- oldWorkDir
