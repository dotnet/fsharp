namespace FSharp.Compiler.Benchmarks

open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

type private SingleFileCompilerConfig =
    {
        Checker : FSharpChecker
        Options : FSharpProjectOptions
    }

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type DecentlySizedStandAloneFileBenchmark() =

    let mutable configOpt : SingleFileCompilerConfig option = None
    let filePath = Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs")

    let getFileSourceText (filePath : string) =
        let text = File.ReadAllText(filePath)
        SourceText.ofString text

    let getConfig () =
        configOpt
        |> Option.defaultWith (fun () -> failwith "Setup not run")

    [<GlobalSetup>]
    member _.Setup() =
        configOpt <-
            match configOpt with
            | Some _ -> configOpt
            | None ->
                let checker = FSharpChecker.Create(projectCacheSize = 200)
                let options =
                    checker.GetProjectOptionsFromScript(filePath, getFileSourceText filePath)
                    |> Async.RunSynchronously
                    |> fst
                {
                    Checker = checker
                    Options = options
                }
                |> Some

    [<Benchmark>]
    member _.Run() =
        let config = getConfig()
        let _, result =
            config.Checker.ParseAndCheckFileInProject(filePath, 0, getFileSourceText filePath, config.Options)
            |> Async.RunSynchronously

        match result with
        | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            if results.Diagnostics.Length > 0 then failwithf $"had errors: %A{results.Diagnostics}"

    [<IterationCleanup>]
    member _.Cleanup() =
        let checker = getConfig().Checker
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
