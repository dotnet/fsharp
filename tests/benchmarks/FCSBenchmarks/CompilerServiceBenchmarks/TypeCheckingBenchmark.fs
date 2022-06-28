namespace FSharp.Compiler.Benchmarks

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Benchmarks.BenchmarkHelpers

[<MemoryDiagnoser>]
type TypeCheckingBenchmark() =
    let mutable checkerOpt = None
    let mutable testFileOpt = None
    
    [<GlobalSetup>]
    member _.Setup() =
        match checkerOpt with
        | None -> checkerOpt <- Some(FSharpChecker.Create(projectCacheSize = 200))
        | _ -> ()

        match testFileOpt with
        | None ->
            let options, _ =
                checkerOpt.Value.GetProjectOptionsFromScript(sourcePath, SourceText.ofString decentlySizedStandAloneFile)
                |> Async.RunImmediate
            testFileOpt <- Some options
        | _ -> ()

    [<Benchmark>]
    member _.Run() =
        match checkerOpt, testFileOpt with
        | None, _ -> failwith "no checker"
        | _, None -> failwith "no test file"
        | Some(checker), Some(options) ->
            let _, result =                                                                
                checker.ParseAndCheckFileInProject(sourcePath, 0, SourceText.ofString decentlySizedStandAloneFile, options)
                |> Async.RunImmediate
            match result with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                if results.Diagnostics.Length > 0 then failwithf $"had errors: %A{results.Diagnostics}"

    [<IterationCleanup>]
    member _.Cleanup() =
        match checkerOpt with
        | None -> failwith "no checker"
        | Some(checker) ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            ClearAllILModuleReaderCache()
