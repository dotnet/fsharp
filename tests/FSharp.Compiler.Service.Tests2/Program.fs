module FSharp.Compiler.Service.Tests2.Program

open System
open FSharp.Compiler
open FSharp.Compiler.Service.Tests
open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

let runCompiler () =
    Environment.CurrentDirectory <- "c:/projekty/fsharp/heuristic/src/Compiler"
    RunCompiler.runCompiler()

type Method =
    | Sequential
    | ParallelFs
    | Graph

let setupOtel () =
    Sdk
        .CreateTracerProviderBuilder()
        .AddSource("fsc")
        .SetResourceBuilder(
            ResourceBuilder
                .CreateDefault()
                .AddService (serviceName = "program", serviceVersion = "42.42.42.44")
        )
        .AddJaegerExporter(fun c ->
            c.BatchExportProcessorOptions.MaxQueueSize <- 10000000
            c.BatchExportProcessorOptions.MaxExportBatchSize <- 10000000
            c.ExportProcessorType <- ExportProcessorType.Simple
            c.MaxPayloadSizeInBytes <- Nullable (1000000000)
        )
        .Build ()

[<EntryPoint>]
let main argv =
    //let args = System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\DiamondArgs.txt")
    
    let parseMode (mode : string) =
        match mode.ToLower() with
        | "sequential" -> Method.Sequential
        | "parallelfs" -> Method.ParallelFs
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
        
    printfn $"WorkingDir = {workingDir}"
    
    let args = System.IO.File.ReadAllLines(path)
    
    let args =
        match mode with
        | Method.Sequential ->
            args
        | Method.ParallelFs ->
            printfn "Using fsParallel"
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
        | Method.Graph ->
            printfn "Using graph"
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Real.CheckMultipleInputsInParallelMy
            Array.append args [|"--test:ParallelCheckingWithSignatureFilesOn"|]
    
    workingDir |> Option.iter (fun dir -> Environment.CurrentDirectory <- dir)
    
    let tracerProvider = setupOtel()
    
    let exit = CommandLineMain.main args
    
    tracerProvider.ForceFlush () |> ignore
    tracerProvider.Dispose()
    exit