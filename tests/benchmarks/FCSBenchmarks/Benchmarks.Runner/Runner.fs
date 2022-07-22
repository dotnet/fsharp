module Benchmarks.Runner

open System
open System.Diagnostics
open System.IO
open FSharp.Compiler.CodeAnalysis
open Benchmarks.Common.Dtos

type FCSBenchmark (config : BenchmarkConfig) =
    let checker = FSharpChecker.Create(projectCacheSize = config.ProjectCacheSize)
        
    let failOnErrors (results : FSharpCheckFileResults) =
        printfn $"{results.Diagnostics.Length} diagnostics calculated:"
        for d in results.Diagnostics do
            printfn $"- {d.Message}"
    
    let performAction (action : BenchmarkAction) =
        let sw = Stopwatch.StartNew()
        let res =
            match action with
            | AnalyseFile x ->
                let result, answer =
                    checker.ParseAndCheckFileInProject(x.FileName, x.FileVersion, FSharp.Compiler.Text.SourceText.ofString x.SourceText, x.Options)
                    |> Async.RunSynchronously
                match answer with
                | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
                | FSharpCheckFileAnswer.Succeeded results ->
                    failOnErrors results
                action, ((result, answer) :> Object)
        printfn $"Performed action {action.GetType()} in {sw.ElapsedMilliseconds}ms"
        res
            
    let cleanCaches () =
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        
    member this.Checker = checker
    member this.PerformAction action = performAction action
    member this.CleanCaches () = cleanCaches

let run (inputs : BenchmarkInputs) =
    let sw = Stopwatch.StartNew()
    let b = FCSBenchmark(inputs.Config)
    let outputs =
        inputs.Actions
        |> List.map b.PerformAction
    printfn $"Performed {outputs.Length} actions in {sw.ElapsedMilliseconds}ms"
    ()

[<EntryPoint>]
let main args =
    match args with
    | [|inputFile|] ->
        let json = File.ReadAllText(inputFile)
        let inputs = deserializeInputs json
        run inputs
        0
    | _ ->
        printfn $"Invalid args: %A{args}. Expected format: 'dotnet run [input file.json]'"
        1
    