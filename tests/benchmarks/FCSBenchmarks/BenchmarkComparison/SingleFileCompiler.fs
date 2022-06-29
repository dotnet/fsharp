// Code in this file supports older versions of the compiler via preprocessor directives using the following DEFINEs:
// - SERVICE_13_0_0
// - SERVICE_30_0_0
// - latest (no DEFINE set)
// The purpose is to allow running the same benchmark old different historical versions for comparison.

namespace HistoricalBenchmark

#if SERVICE_13_0_0
open Microsoft.FSharp.Compiler.SourceCodeServices
#else
#if SERVICE_30_0_0
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
#else
open FSharp.Compiler.CodeAnalysis
#endif
#endif

type private SingleFileCompilerConfig =
    {
        Checker : FSharpChecker
        Options : FSharpProjectOptions
    }

[<RequireQualifiedAccess>]
type OptionsCreationMethod =
    | CmdlineArgs
    | FromScript

/// <summary>Performs compilation (FSharpChecker.ParseAndCheckFileInProject) on the given file</summary>
type SingleFileCompiler(filePath: string, optionsCreationMethod : OptionsCreationMethod) =

    let mutable configOpt : SingleFileCompilerConfig option = None

    member _.Setup() =
        configOpt <-
            match configOpt with
            | Some _ -> configOpt
            | None ->
                let checker = FSharpChecker.Create(projectCacheSize = 200)
                let options =
                    match optionsCreationMethod with
                    | OptionsCreationMethod.CmdlineArgs ->
                        let args = Helpers.makeCmdlineArgsWithSystemReferences filePath
                        checker.GetProjectOptionsFromCommandLineArgs(filePath, args)
                    | OptionsCreationMethod.FromScript ->
                        checker.GetProjectOptionsFromScript(filePath, Helpers.getFileSourceText filePath)
                        |> Async.RunSynchronously
                        |> fst
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
                checker.ParseAndCheckFileInProject(filePath, 0, Helpers.getFileSourceText filePath, options)
                |> Async.RunSynchronously
            match result with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                Helpers.failOnErrors results

    member _.Cleanup() =
        match configOpt with
        | None -> failwith "Setup not run"
        | Some {Checker = checker} ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            FSharp.Compiler.AbstractIL.ILBinaryReader.ClearAllILModuleReaderCache()
