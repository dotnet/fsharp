namespace BenchmarkComparison

open System.IO
open System.Threading.Tasks
#if SERVICE_13_0_0
open Microsoft.FSharp.Compiler.SourceCodeServices
#else
#if SERVICE_30_0_0
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
#else
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
#endif
#endif

[<RequireQualifiedAccess>]
module private Helpers =

    let getFileSourceText (filePath : string) =
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
    
    let makeCmdlineArgsWithSystemReferences (filePath : string) =
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
            
        let refs =
            assemblies
            |> Array.map (fun x ->
                $"-r:{x}"
            )
        [|"--simpleresolution";"--targetprofile:netcore";"--noframework"|]
        |> Array.append refs
        |> Array.append [|filePath|]


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
