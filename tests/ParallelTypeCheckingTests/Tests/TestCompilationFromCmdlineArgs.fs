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
        //@"$CODE_ROOT$\tests\FSharp.Compiler.ComponentTests", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\ComponentTests_args.txt"
        //@"$CODE_ROOT$\src\compiler", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\Tests\FCSArgs.txt"
        @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS_no_fsi.txt", Some 360
        //@"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS_no_fsi.txt", Some 227
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 227
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 239
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 256
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 308
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 407
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", Some 502
        // // @"$CODE_ROOT$\src\compiler", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\Tests\FCS_323.txt"
        // // @"$CODE_ROOT$\src\compiler", @"$CODE_ROOT$\tests\ParallelTypeCheckingTests\Tests\FCS_434.txt"
        // @"$CODE_ROOT$\src\compiler", @"c:\projekty\fsharp\heuristic\tests\ParallelTypeCheckingTests\Tests\FCS.txt", None
    ]

let configs =
    [Method.Sequential; Method.ParallelFs; Method.Graph; Method.Nojaf]
    |> List.allPairs codebases
    |> List.map (fun ((workDir, path, lineLimit : int option), method) -> 
        {
            Path = path
            LineLimit = lineLimit
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
    let {Path = path; LineLimit = lineLimit; Mode = mode; WorkingDir = workingDir} = config
    let args =
        System.IO.File.ReadAllLines(path |> replacePaths)
        |> fun lines -> match lineLimit with Some limit -> Array.take (Math.Min(limit, lines.Length)) lines | None -> lines 
        |> Array.map replacePaths
        
    printfn $"WorkingDir = {workingDir}"
    let args = setupArgsMethod mode args
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- replaceCodeRoot dir)
    args

[<TestCaseSource(nameof(configs))>]
let TestCompilerFromArgs (config : Args) : unit =
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
