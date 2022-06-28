module BenchmarkComparison

open System
open System.IO
open System.Threading.Tasks
open BenchmarkDotNet.Attributes

[<AutoOpen>]
module BenchmarkHelpers =

    type Async with
        static member RunImmediate (computation: Async<'T>, ?cancellationToken ) =
            let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
            let ts = TaskCompletionSource<'T>()
            let task = ts.Task
            Async.StartWithContinuations(
                computation,
                (fun k -> ts.SetResult k),
                (fun exn -> ts.SetException exn),
                (fun _ -> ts.SetCanceled()),
                cancellationToken)
            task.Result

#if SERVICE_13_0_0
    open Microsoft.FSharp.Compiler.SourceCodeServices
#else
#if SERVICE_30_0_0
    open FSharp.Compiler.SourceCodeServices
    open FSharp.Compiler.Text
#else
    open FSharp.Compiler.Diagnostics
    open FSharp.Compiler.CodeAnalysis
    open FSharp.Compiler.Text
#endif
#endif

    type BenchmarkSingleFileCompiler(filePath: string) =

        let mutable checkerOpt = None
        let mutable testFileOpt = None
        let mutable assembliesOpt = None

        let fileText = 
#if SERVICE_13_0_0
            File.ReadAllText(filePath)
#else
            SourceText.ofString(File.ReadAllText(filePath))
#endif
        
        member _.Setup() = 
            match checkerOpt with
            | None -> checkerOpt <- Some(FSharpChecker.Create(projectCacheSize = 200))
            | _ -> ()

            match assembliesOpt with
            | None -> 
                let mainAssemblyLocation = typeof<System.Object>.Assembly.Location
                let frameworkDirectory = Path.GetDirectoryName(mainAssemblyLocation)
                assembliesOpt <- 
                    Directory.EnumerateFiles(frameworkDirectory)
                    |> Seq.filter (fun x ->
                        let name = Path.GetFileName(x)
                        (name.StartsWith("System.") && name.EndsWith(".dll") && not(name.Contains("Native"))) ||
                        name.Contains("netstandard") ||
                        name.Contains("mscorlib")
                    )
                    |> Array.ofSeq
                    |> Array.append [|typeof<Async>.Assembly.Location|]
                    |> Some
            
            | _ -> ()

            match testFileOpt with
            | None ->
                let refs =
                    assembliesOpt.Value
                    |> Array.map (fun x ->
                        $"-r:{x}"
                    )
                let args =
                    Array.append refs [|"--simpleresolution";"--targetprofile:netcore";"--noframework"|]
                let args =
                    Array.append [|filePath|] args
                let options =
                    checkerOpt.Value.GetProjectOptionsFromCommandLineArgs("test.fsproj", args)
                testFileOpt <- Some options
            | _ -> ()

        member _.Run() =
            match checkerOpt, testFileOpt with
            | None, _ -> failwith "no checker"
            | _, None -> failwith "no test file"
            | Some(checker), Some(options) ->
                let _, result =                                                                
                    checker.ParseAndCheckFileInProject(filePath, 0, fileText, options)
                    |> Async.RunImmediate
                match result with
                | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
                | FSharpCheckFileAnswer.Succeeded results ->
#if SERVICE_13_0_0
                    if results.Errors.Length > 0 then failwithf "had errors: %A" results.Errors
#else
#if SERVICE_30_0_0
                    if results.Errors.Length > 0 then failwithf "had errors: %A" results.Errors
#else
                    if results.Diagnostics.Length > 0 then failwithf "had errors: %A" results.Diagnostics
#endif
#endif

        member _.Cleanup() =
            match checkerOpt with
            | None -> failwith "no checker"
            | Some(checker) ->
                checker.InvalidateAll()
                checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

[<MemoryDiagnoser>]
type TypeCheckingBenchmark() =
    let compiler = BenchmarkSingleFileCompiler(Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs"))

    [<GlobalSetup>]
    member _.Setup() =
        compiler.Setup()

    [<Benchmark>]
    member _.Run() =
        compiler.Run()

    [<IterationCleanup>]
    member _.Cleanup() =
        compiler.Cleanup()