module FSharp.Compiler.Service.Tests2.Program
#nowarn "1182"
open System
open FSharp.Compiler
open FSharp.Compiler.Service.Tests
open ParallelTypeCheckingTests.Tests.Utils

let parse (argv: string[]): Args =
    let parseMode (mode : string) =
        match mode.ToLower() with
        | "sequential" -> Method.Sequential
        | "parallelfs" -> Method.ParallelFs
        | "nojaf" -> Method.Nojaf
        | "graph" -> Method.Graph
        | _ -> failwith $"Unrecognised method: {mode}"
    
    let path, mode, workingDir =
        match argv with
        | [|path|] ->
            path, Method.Sequential, None
        | [|path; mode|] ->
            path, parseMode mode, None
        | [|path; mode; workingDir|] ->
            path, parseMode mode, Some workingDir
        | _ -> failwith "Invalid args - use 'args_path [fs-parallel]"
    
    {
        Path = path
        Mode = mode
        WorkingDir = workingDir
    }

let setupArgsMethod (method: Method) (args: string[]): string[] =
    printfn $"Method: {method}"
    match method with
        | Method.Sequential ->
            args
        | Method.ParallelFs ->
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Graph ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Real.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Nojaf ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Nojaf.CheckMultipleInputsInParallel
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]

let setupParsed config =
    let {Path = path; Mode = mode; WorkingDir = workingDir} = config
    let args = System.IO.File.ReadAllLines(path)
    printfn $"WorkingDir = {workingDir}"
    let args = setupArgsMethod mode args
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- dir)
    args

[<EntryPoint>]
let main argv =
    //let args = System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\DiamondArgs.txt")
    let config = parse argv
    let args = setupParsed config
    use tracerProvider = setupOtel()
    CommandLineMain.main args