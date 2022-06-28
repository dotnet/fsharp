namespace FSharp.Compiler.Benchmarks

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
open FSharp.Compiler.Benchmarks.BenchmarkHelpers

[<MemoryDiagnoser>]
type CompilerServiceBenchmarks() =
    let mutable checkerOpt = None
    let mutable sourceOpt = None
    let mutable assembliesOpt = None
    let mutable decentlySizedStandAloneFileCheckResultOpt = None

    let parsingOptions =
        {
            SourceFiles = [|"CheckExpressions.fs"|]
            ConditionalDefines = []
            DiagnosticOptions = FSharpDiagnosticOptions.Default
            LangVersionText = "default"
            IsInteractive = false
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
        match checkerOpt with
        | None ->
            checkerOpt <- Some(FSharpChecker.Create(projectCacheSize = 200))
        | _ -> ()

        match sourceOpt with
        | None ->
            sourceOpt <- Some <| FSharpSourceText.From(File.OpenRead("""..\..\..\..\..\..\..\..\..\src\Compiler\CheckExpressions.fs"""), Encoding.Default, FSharpSourceHashAlgorithm.Sha1, true)
        | _ -> ()

        match assembliesOpt with
        | None -> 
            assembliesOpt <- 
                System.AppDomain.CurrentDomain.GetAssemblies()
                |> Array.map (fun x -> (x.Location))
                |> Some
        
        | _ -> ()

        match decentlySizedStandAloneFileCheckResultOpt with
        | None ->
            let options, _ =
                checkerOpt.Value.GetProjectOptionsFromScript(sourcePath, SourceText.ofString decentlySizedStandAloneFile)
                |> Async.RunImmediate
            let _, checkResult =                                                                
                checkerOpt.Value.ParseAndCheckFileInProject(sourcePath, 0, SourceText.ofString decentlySizedStandAloneFile, options)
                |> Async.RunImmediate
            decentlySizedStandAloneFileCheckResultOpt <- Some checkResult
        | _ -> ()

    [<Benchmark>]
    member _.ParsingTypeCheckerFs() =
        match checkerOpt, sourceOpt with
        | None, _ -> failwith "no checker"
        | _, None -> failwith "no source"
        | Some(checker), Some(source) ->
            let results = checker.ParseFile("CheckExpressions.fs", source.ToFSharpSourceText(), parsingOptions) |> Async.RunImmediate
            if results.ParseHadErrors then failwithf "parse had errors: %A" results.Diagnostics

    [<IterationCleanup(Target = "ParsingTypeCheckerFs")>]
    member _.ParsingTypeCheckerFsSetup() =
        match checkerOpt with
        | None -> failwith "no checker"
        | Some(checker) ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            checker.ParseFile("dummy.fs", SourceText.ofString "dummy", parsingOptions) |> Async.RunImmediate |> ignore
            ClearAllILModuleReaderCache()

    [<Benchmark>]
    member _.ILReading() =
        match assembliesOpt with
        | None -> failwith "no assemblies"
        | Some(assemblies) ->
            // We try to read most of everything in the assembly that matter, mainly types with their properties, methods, and fields.
            // CustomAttrs and SecurityDecls are lazy until you call them, so we call them here for benchmarking.
            for fileName in assemblies do
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

        match checkerOpt with
        | None -> failwith "no checker"
        | Some checker ->
            let parseResult, checkResult =                                                                
                checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString (File.ReadAllText(file)), options)
                |> Async.RunImmediate

            if parseResult.Diagnostics.Length > 0 then
                failwithf "%A" parseResult.Diagnostics

            match checkResult with
            | FSharpCheckFileAnswer.Aborted -> failwith "aborted"
            | FSharpCheckFileAnswer.Succeeded checkFileResult ->

                if checkFileResult.Diagnostics.Length > 0 then
                    failwithf "%A" checkFileResult.Diagnostics

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

        match checkerOpt with
        | None -> failwith "no checker"
        | Some checker ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            ClearAllILModuleReaderCache()

    [<Benchmark>]
    member _.SimplifyNames() =
        match decentlySizedStandAloneFileCheckResultOpt with
        | Some checkResult ->
            match checkResult with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                let sourceLines = decentlySizedStandAloneFile.Split ([|"\r\n"; "\n"; "\r"|], StringSplitOptions.None)
                let ranges = SimplifyNames.getSimplifiableNames(results, fun lineNum -> sourceLines.[Line.toZ lineNum]) |> Async.RunImmediate
                ignore ranges                
        | _ -> failwith "oopsie"

    [<Benchmark>]
    member _.UnusedOpens() =
        match decentlySizedStandAloneFileCheckResultOpt with
        | Some checkResult ->
            match checkResult with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                let sourceLines = decentlySizedStandAloneFile.Split ([|"\r\n"; "\n"; "\r"|], StringSplitOptions.None)
                let decls = UnusedOpens.getUnusedOpens(results, fun lineNum -> sourceLines.[Line.toZ lineNum]) |> Async.RunImmediate
                ignore decls              
        | _ -> failwith "oopsie"

    [<Benchmark>]
    member _.UnusedDeclarations() =
        match decentlySizedStandAloneFileCheckResultOpt with
        | Some checkResult ->
            match checkResult with
            | FSharpCheckFileAnswer.Aborted -> failwith "checker aborted"
            | FSharpCheckFileAnswer.Succeeded results ->
                let decls = UnusedDeclarations.getUnusedDeclarations(results, true) |> Async.RunImmediate
                ignore decls // should be 16                
        | _ -> failwith "oopsie"