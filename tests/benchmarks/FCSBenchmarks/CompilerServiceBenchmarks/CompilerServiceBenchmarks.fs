﻿namespace FSharp.Compiler.Benchmarks

open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Benchmarks
open Microsoft.CodeAnalysis.Text

type private Config =
    {
        Checker : FSharpChecker
        Source : SourceText
        Assemblies : string[]
        CheckResult : FSharpCheckFileAnswer
        DecentlySizedFileSource : string
    }

[<AutoOpen>]
module private Helpers =
        
    let createProject name referencedProjects =
        let tmpPath = Path.GetTempPath()
        let file = Path.Combine(tmpPath, Path.ChangeExtension(name, ".fs"))
        {
            ProjectFileName = Path.Combine(tmpPath, Path.ChangeExtension(name, ".dll"))
            ProjectId = None
            SourceFiles = [|file|]
            OtherOptions = 
                Array.append [|"--optimize+"; "--target:library" |] (referencedProjects |> Array.ofList |> Array.map (fun x -> "-r:" + x.ProjectFileName))
            ReferencedProjects =
                referencedProjects
                |> List.map (fun x -> FSharpReferencedProject.CreateFSharp (x.ProjectFileName, x))
                |> Array.ofList
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = Some 0L (* set the stamp to 0L on each run so we don't evaluate the whole project again *)
        }

    let generateSourceCode moduleName =
        $"""
module Benchmark.%s{moduleName}

type %s{moduleName} =

    val X : int

    val Y : int

    val Z : int

let function%s{moduleName} (x: %s{moduleName}) =
    let x = 1
    let y = 2
    let z = x + y
    z"""


[<MemoryDiagnoser>]
type CompilerServiceBenchmarks() =
    let mutable configOpt : Config option = None
    let sourcePath = Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs")
    
    let getConfig () =
        configOpt
        |> Option.defaultWith (fun () -> failwith "Setup not run")
        
    let parsingOptions =
        {
            SourceFiles = [|"CheckExpressions.fs"|]
            ConditionalDefines = []
            DiagnosticOptions = FSharpDiagnosticOptions.Default
            LangVersionText = "default"
            IsInteractive = false
            ApplyLineDirectives = false
            IndentationAwareSyntax = None
            CompilingFSharpCore = false
            IsExe = false
        }

    let readerOptions =
        {
            pdbDirPath = None
            reduceMemoryUsage = ReduceMemoryFlag.No
            metadataOnly = MetadataOnlyFlag.Yes
            tryGetMetadataSnapshot = fun _ -> None
        }

    [<GlobalSetup>]
    member _.Setup() =
        configOpt <-
            match configOpt with
            | Some _ -> configOpt
            | None ->
                let checker = FSharpChecker.Create(projectCacheSize = 200)
                let source = FSharpSourceText.From(File.OpenRead("""..\..\..\..\..\..\..\..\..\src\Compiler\Checking\CheckExpressions.fs"""), Encoding.Default, FSharpSourceHashAlgorithm.Sha1, true)
                let assemblies = 
                    AppDomain.CurrentDomain.GetAssemblies()
                    |> Array.map (fun x -> x.Location)
                let decentlySizedStandAloneFile = File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, sourcePath))
                let options, _ =
                    checker.GetProjectOptionsFromScript(sourcePath, SourceText.ofString decentlySizedStandAloneFile)
                    |> Async.RunSynchronously
                let _, checkResult =                                                                
                    checker.ParseAndCheckFileInProject(sourcePath, 0, SourceText.ofString decentlySizedStandAloneFile, options)
                    |> Async.RunSynchronously
                {
                    Checker = checker
                    Source = source
                    Assemblies = assemblies
                    CheckResult = checkResult
                    DecentlySizedFileSource = decentlySizedStandAloneFile
                }
                |> Some
    
    [<Benchmark>]
    member _.ParsingTypeCheckerFs() =
        let config = getConfig()
        let results = config.Checker.ParseFile("CheckExpressions.fs", config.Source |> SourceText.toFSharpSourceText, parsingOptions) |> Async.RunSynchronously
        if results.ParseHadErrors then failwithf $"parse had errors: %A{results.Diagnostics}"

    [<IterationCleanup(Target = "ParsingTypeCheckerFs")>]
    member _.ParsingTypeCheckerFsSetup() =
        let checker = getConfig().Checker
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        checker.ParseFile("dummy.fs", SourceText.ofString "dummy", parsingOptions) |> Async.RunSynchronously |> ignore
        ClearAllILModuleReaderCache()

    [<Benchmark>]
    member _.ILReading() =
        let config = getConfig()
        // We try to read most of everything in the assembly that matter, mainly types with their properties, methods, and fields.
        // CustomAttrs and SecurityDecls are lazy until you call them, so we call them here for benchmarking.
        for fileName in config.Assemblies do
            let reader = OpenILModuleReader fileName readerOptions

            let ilModuleDef = reader.ILModuleDef

            let ilAssemblyManifest = ilModuleDef.Manifest.Value

            ilAssemblyManifest.CustomAttrs |> ignore
            ilAssemblyManifest.SecurityDecls |> ignore
            for x in ilAssemblyManifest.ExportedTypes.AsList() do
                x.CustomAttrs |> ignore

            ilModuleDef.CustomAttrs |> ignore
            for ilTypeDef in ilModuleDef.TypeDefs.AsArray() do
                ilTypeDef.CustomAttrs |> ignore
                ilTypeDef.SecurityDecls |> ignore

                for ilMethodDef in ilTypeDef.Methods.AsArray() do
                    ilMethodDef.CustomAttrs |> ignore
                    ilMethodDef.SecurityDecls |> ignore

                for ilFieldDef in ilTypeDef.Fields.AsList() do
                    ilFieldDef.CustomAttrs |> ignore

                for ilPropertyDef in ilTypeDef.Properties.AsList() do
                    ilPropertyDef.CustomAttrs |> ignore

    [<IterationCleanup(Target = "ILReading")>]
    member _.ILReadingSetup() =
        // With caching, performance increases an order of magnitude when re-reading an ILModuleReader.
        // Clear it for benchmarking.
        ClearAllILModuleReaderCache()

    member val TypeCheckFileWith100ReferencedProjectsOptions =
        createProject "MainProject"
            [ for i = 1 to 100 do
                 createProject ("ReferencedProject" + string i) []
            ]

    member this.TypeCheckFileWith100ReferencedProjectsRun() =
        let options = this.TypeCheckFileWith100ReferencedProjectsOptions
        let file = options.SourceFiles.[0]
        let checker = getConfig().Checker
        let parseResult, checkResult =                                                                
            checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString (File.ReadAllText(file)), options)
            |> Async.RunSynchronously

        if parseResult.Diagnostics.Length > 0 then
            failwithf $"%A{parseResult.Diagnostics}"

        match checkResult with
        | FSharpCheckFileAnswer.Aborted -> failwith "aborted"
        | FSharpCheckFileAnswer.Succeeded checkFileResult ->

            if checkFileResult.Diagnostics.Length > 0 then
                failwithf $"%A{checkFileResult.Diagnostics}"

    [<IterationSetup(Target = "TypeCheckFileWith100ReferencedProjects")>]
    member this.TypeCheckFileWith100ReferencedProjectsSetup() =
        for file in this.TypeCheckFileWith100ReferencedProjectsOptions.SourceFiles do
            File.WriteAllText(file, generateSourceCode (Path.GetFileNameWithoutExtension(file)))

        for proj in this.TypeCheckFileWith100ReferencedProjectsOptions.ReferencedProjects do
            match proj with
            | FSharpReferencedProject.FSharpReference(_, referencedProjectOptions) ->
                for file in referencedProjectOptions.SourceFiles do
                    File.WriteAllText(file, generateSourceCode (Path.GetFileNameWithoutExtension(file)))
            | _ -> ()

        this.TypeCheckFileWith100ReferencedProjectsRun()

    [<Benchmark>]
    member this.TypeCheckFileWith100ReferencedProjects() =
        // Because the checker's projectcachesize is set to 200, this should be fast.
        // If set to 3, it will be almost as slow as re-evaluating all project and it's projects references on setup; this could be a bug or not what we want.
        this.TypeCheckFileWith100ReferencedProjectsRun()

    member val TypeCheckFileWithNoReferencesOptions = createProject "MainProject" []

    [<IterationCleanup(Target = "TypeCheckFileWith100ReferencedProjects")>]
    member this.TypeCheckFileWith100ReferencedProjectsCleanup() =
        for file in this.TypeCheckFileWith100ReferencedProjectsOptions.SourceFiles do
            try File.Delete(file) with | _ -> ()

        for proj in this.TypeCheckFileWith100ReferencedProjectsOptions.ReferencedProjects do
            match proj with
            | FSharpReferencedProject.FSharpReference(_, referencedProjectOptions) ->
                for file in referencedProjectOptions.SourceFiles do
                    try File.Delete(file) with | _ -> ()
            | _ -> ()

        let checker = getConfig().Checker
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        ClearAllILModuleReaderCache()

    [<Benchmark>]
    member _.SimplifyNames() =
        let checkResult = getConfig().CheckResult
        let source = getConfig().DecentlySizedFileSource
        match checkResult with
        | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let sourceLines = source.Split ([|"\r\n"; "\n"; "\r"|], StringSplitOptions.None)
            let ranges = SimplifyNames.getSimplifiableNames(results, fun lineNum -> sourceLines.[Line.toZ lineNum]) |> Async.RunSynchronously
            ignore ranges                

    [<Benchmark>]
    member _.UnusedOpens() =
        let checkResult = getConfig().CheckResult
        let source = getConfig().DecentlySizedFileSource
        match checkResult with
        | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let sourceLines = source.Split ([|"\r\n"; "\n"; "\r"|], StringSplitOptions.None)
            let decls = UnusedOpens.getUnusedOpens(results, fun lineNum -> sourceLines.[Line.toZ lineNum]) |> Async.RunSynchronously
            ignore decls              

    [<Benchmark>]
    member _.UnusedDeclarations() =
        let checkResult = getConfig().CheckResult
        match checkResult with
        | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let decls = UnusedDeclarations.getUnusedDeclarations(results, true) |> Async.RunSynchronously
            ignore decls // should be 16                
