module BenchmarkHelpers

open System.IO
open System.Threading.Tasks

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
#elif SERVICE_30_0_0
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
#else
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
#endif

type Config =
    {
        Checker : FSharpChecker
        Options : FSharpProjectOptions
    }

[<AutoOpen>]
module Helpers =

    let getFileText (filePath : string) =
        let text = File.ReadAllText(filePath)
#if SERVICE_13_0_0
        text
#else
        SourceText.ofString text
#endif

    let failOnErrors (results : FSharpCheckFileResults) =
#if SERVICE_13_0_0 || SERVICE_30_0_0 
        if results.Errors.Length > 0 then failwithf "had errors: %A" results.Errors
#else
        if results.Diagnostics.Length > 0 then failwithf $"had errors: %A{results.Diagnostics}"
#endif
    
    let makeOptionsForFile (checker : FSharpChecker) (filePath : string) =
        let assemblies =
            let mainAssemblyLocation = typeof<System.Object>.Assembly.Location
            let frameworkDirectory = Path.GetDirectoryName(mainAssemblyLocation)
            Directory.EnumerateFiles(frameworkDirectory)
            |> Seq.filter (fun x ->
                let name = Path.GetFileName(x)
                (name.StartsWith("System.") && name.EndsWith(".dll") && not(name.Contains("Native"))) ||
                name.Contains("netstandard") ||
                name.Contains("mscorlib")
            )
            |> Array.ofSeq
            |> Array.append [|typeof<Async>.Assembly.Location|]
        let args =
            let refs =
                assemblies
                |> Array.map (fun x ->
                    $"-r:{x}"
                )
            [|"--simpleresolution";"--targetprofile:netcore";"--noframework"|]
            |> Array.append refs
            |> Array.append [|filePath|]
        checker.GetProjectOptionsFromCommandLineArgs(filePath, args)

/// <summary>Performs compilation (FSharpChecker.ParseAndCheckFileInProject) on the given file</summary>
type BenchmarkSingleFileCompiler(filePath: string) =

    let mutable configOpt : Config option = None    

    member _.Setup() =
        configOpt <-
            match configOpt with
            | Some _ -> configOpt
            | None ->
                let checker = FSharpChecker.Create(projectCacheSize = 200)
                let options = makeOptionsForFile checker filePath
                {
                    Checker = checker
                    Options = options
                }
                |> Some

    member _.Run() =
        match configOpt with
        | None -> failwith "Setup not run"
        | Some {Checker = checker; Options = options} ->
            let _, result =                                                                
                checker.ParseAndCheckFileInProject(filePath, 0, getFileText filePath, options)
                |> Async.RunImmediate
            match result with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                failOnErrors results

    member _.Cleanup() =
        match configOpt with
        | None -> failwith "Setup not run"
        | Some {Checker = checker} ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            FSharp.Compiler.AbstractIL.ILBinaryReader.ClearAllILModuleReaderCache()
