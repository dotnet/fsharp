namespace FSharp.Compiler.CodeAnalysis

open FSharp.Compiler.Text
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.Symbols
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open System
open FSharp.Compiler
open Internal.Utilities.Collections
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.Text.Range
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.ConstraintSolver
open System.Diagnostics
open System.IO
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Xml
open FSharp.Compiler.CompilerImports



type internal FSharpFile = {
    Range: range
    Source: FSharpFileSnapshot
    IsLastCompiland: bool
    IsExe: bool
}

/// Things we need to start parsing and checking files for a given project snapshot
type BootstrapInfo = {
    TcConfig: TcConfig
    SourceFiles: FSharpFile list
}


type internal TransparentCompiler
    (
        legacyReferenceResolver,
        projectCacheSize,
        keepAssemblyContents,
        keepAllBackgroundResolutions,
        tryGetMetadataSnapshot,
        suggestNamesForErrors,
        keepAllBackgroundSymbolUses,
        enableBackgroundItemKeyStoreAndSemanticClassification,
        enablePartialTypeChecking,
        parallelReferenceResolution,
        captureIdentifiersWhenParsing,
        getSource: (string -> ISourceText option) option,
        useChangeNotifications,
        useSyntaxTreeCache
    ) =

    // Is having just one of these ok?
    let lexResourceManager = Lexhelp.LexResourceManager()

    let ParseFileCache = AsyncMemoize()
    let ParseAndCheckFileInProjectCache = AsyncMemoize()
    let FrameworkImportsCache = AsyncMemoize()

    // use this to process not-yet-implemented tasks
    let backgroundCompiler =
        BackgroundCompiler(
            legacyReferenceResolver,
            projectCacheSize,
            keepAssemblyContents,
            keepAllBackgroundResolutions,
            tryGetMetadataSnapshot,
            suggestNamesForErrors,
            keepAllBackgroundSymbolUses,
            enableBackgroundItemKeyStoreAndSemanticClassification,
            enablePartialTypeChecking,
            parallelReferenceResolution,
            captureIdentifiersWhenParsing,
            getSource,
            useChangeNotifications,
            useSyntaxTreeCache
        )
        :> IBackgroundCompiler

    let getProjectReferences (project: FSharpProjectSnapshot) userOpName =
        [
            for r in project.ReferencedProjects do

                match r with
                | FSharpReferencedProjectSnapshot.FSharpReference (nm, opts) ->
                    // Don't use cross-project references for FSharp.Core, since various bits of code
                    // require a concrete FSharp.Core to exist on-disk. The only solutions that have
                    // these cross-project references to FSharp.Core are VisualFSharp.sln and FSharp.sln. The ramification
                    // of this is that you need to build FSharp.Core to get intellisense in those projects.

                    if
                        (try
                            Path.GetFileNameWithoutExtension(nm)
                         with _ ->
                             "")
                        <> GetFSharpCoreLibraryName()
                    then
                        { new IProjectReference with
                            member x.EvaluateRawContents() =
                                node {
                                    Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "GetAssemblyData", nm)
                                    return! backgroundCompiler.GetAssemblyData(opts.ToOptions(), userOpName + ".CheckReferencedProject(" + nm + ")")
                                }

                            member x.TryGetLogicalTimeStamp(cache) =
                                // TODO:
                                None

                            member x.FileName = nm
                        }
        ]


    let ComputeFrameworkImports (tcConfig: TcConfig) _key = node {
        let tcConfigP = TcConfigProvider.Constant tcConfig
        return! TcImports.BuildFrameworkTcImports (tcConfigP, frameworkDLLs, nonFrameworkResolutions)
    }


    let ComputeBootstapInfo (projectSnapshot: FSharpProjectSnapshot) =
        node {

            let useSimpleResolutionSwitch = "--simpleresolution"
            let commandLineArgs = projectSnapshot.OtherOptions
            let defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir
            let useScriptResolutionRules = projectSnapshot.UseScriptResolutionRules

            let projectReferences = getProjectReferences projectSnapshot "ComputeBootstapInfo"
            let sourceFiles = projectSnapshot.SourceFileNames

            // TODO: script support
            let loadClosureOpt: LoadClosure option = None

            let tcConfigB, sourceFiles =

                let getSwitchValue switchString =
                    match commandLineArgs |> List.tryFindIndex(fun s -> s.StartsWithOrdinal switchString) with
                    | Some idx -> Some(commandLineArgs[idx].Substring(switchString.Length))
                    | _ -> None

                let sdkDirOverride =
                    match loadClosureOpt with
                    | None -> None
                    | Some loadClosure -> loadClosure.SdkDirOverride

                // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB =
                    TcConfigBuilder.CreateNew(legacyReferenceResolver,
                         defaultFSharpBinariesDir,
                         implicitIncludeDir=projectSnapshot.ProjectDirectory,
                         reduceMemoryUsage=ReduceMemoryFlag.Yes,
                         isInteractive=useScriptResolutionRules,
                         isInvalidationSupported=true,
                         defaultCopyFSharpCore=CopyFSharpCoreFlag.No,
                         tryGetMetadataSnapshot=tryGetMetadataSnapshot,
                         sdkDirOverride=sdkDirOverride,
                         rangeForErrors=range0)

                tcConfigB.primaryAssembly <-
                    match loadClosureOpt with
                    | None -> PrimaryAssembly.Mscorlib
                    | Some loadClosure ->
                        if loadClosure.UseDesktopFramework then
                            PrimaryAssembly.Mscorlib
                        else
                            PrimaryAssembly.System_Runtime

                tcConfigB.resolutionEnvironment <- (LegacyResolutionEnvironment.EditingOrCompilation true)

                tcConfigB.conditionalDefines <-
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define :: tcConfigB.conditionalDefines

                tcConfigB.projectReferences <- projectReferences

                tcConfigB.useSimpleResolution <- (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

                // Apply command-line arguments and collect more source files if they are in the arguments
                let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, commandLineArgs)

                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false

                tcConfigB.xmlDocInfoLoader <-
                    { new IXmlDocumentationInfoLoader with
                        /// Try to load xml documentation associated with an assembly by the same file path with the extension ".xml".
                        member _.TryLoad(assemblyFileName) =
                            let xmlFileName = Path.ChangeExtension(assemblyFileName, ".xml")

                            // REVIEW: File IO - Will eventually need to change this to use a file system interface of some sort.
                            XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
                    }
                    |> Some

                tcConfigB.parallelReferenceResolution <- parallelReferenceResolution
                tcConfigB.captureIdentifiersWhenParsing <- captureIdentifiersWhenParsing

                tcConfigB, sourceFilesNew

            // If this is a builder for a script, re-apply the settings inferred from the
            // script and its load closure to the configuration.
            //
            // NOTE: it would probably be cleaner and more accurate to re-run the load closure at this point.
            let setupConfigFromLoadClosure () =
                match loadClosureOpt with
                | Some loadClosure ->
                    let dllReferences =
                        [for reference in tcConfigB.referencedDLLs do
                            // If there's (one or more) resolutions of closure references then yield them all
                            match loadClosure.References  |> List.tryFind (fun (resolved, _)->resolved=reference.Text) with
                            | Some (resolved, closureReferences) ->
                                for closureReference in closureReferences do
                                    yield AssemblyReference(closureReference.originalReference.Range, resolved, None)
                            | None -> yield reference]
                    tcConfigB.referencedDLLs <- []
                    tcConfigB.primaryAssembly <- (if loadClosure.UseDesktopFramework then PrimaryAssembly.Mscorlib else PrimaryAssembly.System_Runtime)
                    // Add one by one to remove duplicates
                    dllReferences |> List.iter (fun dllReference ->
                        tcConfigB.AddReferencedAssemblyByPath(dllReference.Range, dllReference.Text))
                    tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
                | None -> ()

            setupConfigFromLoadClosure()

            let tcConfig = TcConfig.Create(tcConfigB, validate=true)
            let outfile, _, assemblyName = tcConfigB.DecideNames sourceFiles

            // Resolve assemblies and create the framework TcImports. This is done when constructing the
            // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are
            // included in these references.
            let! tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences = frameworkTcImportsCache.Get(tcConfig)

            // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
            // This is ok because not much can actually go wrong here.
            let diagnosticsLogger = CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)
            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

            // Get the names and time stamps of all the non-framework referenced assemblies, which will act
            // as inputs to one of the nodes in the build.
            //
            // This operation is done when constructing the builder itself, rather than as an incremental task.
            let nonFrameworkAssemblyInputs =
                // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
                // This is ok because not much can actually go wrong here.
                let diagnosticsLogger = CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)
                // Return the disposable object that cleans up
                use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

                [ for r in nonFrameworkResolutions do
                    let fileName = r.resolvedPath
                    yield (Choice1Of2 fileName, (fun (cache: TimeStampCache) -> cache.GetFileTimeStamp fileName))

                  for pr in projectReferences  do
                    yield Choice2Of2 pr, (fun (cache: TimeStampCache) -> cache.GetProjectReferenceTimeStamp pr) ]

            // Start importing

            let tcConfigP = TcConfigProvider.Constant tcConfig
            let beforeFileChecked = Event<string>()
            let fileChecked = Event<string>()

#if !NO_TYPEPROVIDERS
            let importsInvalidatedByTypeProvider = Event<unit>()
#endif

            // Check for the existence of loaded sources and prepend them to the sources list if present.
            let sourceFiles = tcConfig.GetAvailableLoadedSources() @ (sourceFiles |>List.map (fun s -> rangeStartup, s))

            // Mark up the source files with an indicator flag indicating if they are the last source file in the project
            let sourceFiles =
                let flags, isExe = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
                ((sourceFiles, flags) ||> List.map2 (fun (m, nm) flag -> (m, nm, (flag, isExe))))

            let basicDependencies =
                [ for UnresolvedAssemblyReference(referenceText, _)  in unresolvedReferences do
                    // Exclude things that are definitely not a file name
                    if not(FileSystem.IsInvalidPathShim referenceText) then
                        let file = if FileSystem.IsPathRootedShim referenceText then referenceText else Path.Combine(projectDirectory, referenceText)
                        yield file

                  for r in nonFrameworkResolutions do
                        yield  r.resolvedPath  ]

            let allDependencies =
                [| yield! basicDependencies
                   for _, f, _ in sourceFiles do
                        yield f |]

            // For scripts, the dependency provider is already available.
            // For projects create a fresh one for the project.
            let dependencyProvider =
                match dependencyProvider with
                | None -> new DependencyProvider()
                | Some dependencyProvider -> dependencyProvider

            let defaultTimeStamp = DateTime.UtcNow

            let! initialBoundModel = 
                CombineImportedAssembliesTask(
                    assemblyName,
                    tcConfig,
                    tcConfigP,
                    tcGlobals,
                    frameworkTcImports,
                    nonFrameworkResolutions,
                    unresolvedReferences,
                    dependencyProvider,
                    loadClosureOpt,
                    basicDependencies,
                    keepAssemblyContents,
                    keepAllBackgroundResolutions,
                    keepAllBackgroundSymbolUses,
                    enableBackgroundItemKeyStoreAndSemanticClassification,
                    enablePartialTypeChecking,
                    beforeFileChecked,
                    fileChecked
#if !NO_TYPEPROVIDERS
                    ,importsInvalidatedByTypeProvider
#endif
                )

            let getFSharpSource fileName =
                getSource
                |> Option.map(fun getSource ->
                    let timeStamp = DateTime.UtcNow
                    let getTimeStamp = fun () -> timeStamp
                    let getSourceText() = getSource fileName
                    FSharpSource.Create(fileName, getTimeStamp, getSourceText))
                |> Option.defaultWith(fun () -> FSharpSource.CreateFromFile(fileName))

            let sourceFiles =
                sourceFiles
                |> List.map (fun (m, fileName, isLastCompiland) -> 
                    { Range = m; Source = getFSharpSource fileName; Flags = isLastCompiland } )

            return (), ()
        }


    let ComputeParseFile (file: FSharpFile) (projectSnapshot: FSharpProjectSnapshot) bootstrapInfo userOpName _key = node {

        let parsingOptions =
            FSharpParsingOptions.FromTcConfig(
                bootstrapInfo.TcConfig,
                projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName) |> Array.ofSeq,
                projectSnapshot.UseScriptResolutionRules
            )

        // TODO: what is this?
        // GraphNode.SetPreferredUILang tcPrior.TcConfig.preferredUiLang

        let! sourceText = file.Source.GetSource() |> NodeCode.AwaitTask

        return ParseAndCheckFile.parseFile (
            sourceText,
            file.Source.FileName,
            parsingOptions,
            userOpName,
            suggestNamesForErrors,
            captureIdentifiersWhenParsing
        )

    }


    let ComputeParseAndCheckFileInProject (fileName: string) (projectSnapshot: FSharpProjectSnapshot) userOpName _key =
        node {

            let! bootstrapInfoOpt, creationDiags = ComputeBootstapInfo projectSnapshot // probably cache

            match bootstrapInfoOpt with
            | None ->
                let parseTree = EmptyParsedInput(fileName, (false, false))
                let parseResults = FSharpParseFileResults(creationDiags, parseTree, true, [||])
                return (parseResults, FSharpCheckFileAnswer.Aborted)

            | Some bootstrapInfo ->

                let file = bootstrapInfo.SourceFiles |> List.find (fun f -> f.Source.FileName = fileName)
                let! parseDiagnostics, parseTree, anyErrors = ParseFileCache.Get(file.Source.Key, ComputeParseFile file projectSnapshot bootstrapInfo userOpName)

                // TODO: check if we really need this in parse results
                let dependencyFiles = [||]

                let parseResults =
                    FSharpParseFileResults(parseDiagnostics, parseTree, anyErrors, dependencyFiles)

                let! checkResults =
                    bc.CheckOneFileImpl(
                        parseResults,
                        sourceText,
                        fileName,
                        options,
                        fileVersion,
                        builder,
                        tcPrior,
                        tcInfo,
                        creationDiags
                    )

                return (parseResults, checkResults)
        }

    member _.ParseAndCheckFileInProject
        (
            fileName: string,
            projectSnapshot: FSharpProjectSnapshot,
            userOpName: string
        ) : NodeCode<FSharpParseFileResults * FSharpCheckFileAnswer> = node {
            ignore userOpName // TODO
            let key = fileName, projectSnapshot.Key
            return! ParseAndCheckFileInProjectCache.Get(key, ComputeParseAndCheckFileInProject fileName projectSnapshot)
        }


    interface IBackgroundCompiler with
        member this.BeforeBackgroundFileCheck: IEvent<string * FSharpProjectOptions> =
            backgroundCompiler.BeforeBackgroundFileCheck

        member _.CheckFileInProject
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpCheckFileAnswer> =
            backgroundCompiler.CheckFileInProject(parseResults, fileName, fileVersion, sourceText, options, userOpName)

        member _.CheckFileInProjectAllowingStaleCachedResults
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpCheckFileAnswer option> =
            backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(parseResults, fileName, fileVersion, sourceText, options, userOpName)

        member _.ClearCache(options: seq<FSharpProjectOptions>, userOpName: string) : unit = backgroundCompiler.ClearCache(options, userOpName)
        member _.ClearCaches() : unit = backgroundCompiler.ClearCaches()
        member _.DownsizeCaches() : unit = backgroundCompiler.DownsizeCaches()
        member _.FileChecked: IEvent<string * FSharpProjectOptions> = backgroundCompiler.FileChecked
        member _.FileParsed: IEvent<string * FSharpProjectOptions> = backgroundCompiler.FileParsed

        member _.FindReferencesInFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                symbol: FSharpSymbol,
                canInvalidateProject: bool,
                userOpName: string
            ) : NodeCode<seq<range>> =
            backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)

        member _.FrameworkImportsCache: FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

        member _.GetAssemblyData(options: FSharpProjectOptions, userOpName: string) : NodeCode<ProjectAssemblyDataResult> =
            backgroundCompiler.GetAssemblyData(options, userOpName)

        member _.GetBackgroundCheckResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults * FSharpCheckFileResults> =
            backgroundCompiler.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName)

        member _.GetBackgroundParseResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults> =
            backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

        member _.GetCachedCheckFileResult
            (
                builder: IncrementalBuilder,
                fileName: string,
                sourceText: ISourceText,
                options: FSharpProjectOptions
            ) : NodeCode<(FSharpParseFileResults * FSharpCheckFileResults) option> =
            backgroundCompiler.GetCachedCheckFileResult(builder, fileName, sourceText, options)

        member _.GetProjectOptionsFromScript
            (
                fileName: string,
                sourceText: ISourceText,
                previewEnabled: bool option,
                loadedTimeStamp: DateTime option,
                otherFlags: string array option,
                useFsiAuxLib: bool option,
                useSdkRefs: bool option,
                sdkDirOverride: string option,
                assumeDotNetFramework: bool option,
                optionsStamp: int64 option,
                userOpName: string
            ) : Async<FSharpProjectOptions * FSharpDiagnostic list> =
            backgroundCompiler.GetProjectOptionsFromScript(
                fileName,
                sourceText,
                previewEnabled,
                loadedTimeStamp,
                otherFlags,
                useFsiAuxLib,
                useSdkRefs,
                sdkDirOverride,
                assumeDotNetFramework,
                optionsStamp,
                userOpName
            )

        member _.GetSemanticClassificationForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<EditorServices.SemanticClassificationView option> =
            backgroundCompiler.GetSemanticClassificationForFile(fileName, options, userOpName)

        member _.InvalidateConfiguration(options: FSharpProjectOptions, userOpName: string) : unit =
            backgroundCompiler.InvalidateConfiguration(options, userOpName)

        member _.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, userOpName: string) : NodeCode<unit> =
            backgroundCompiler.NotifyFileChanged(fileName, options, userOpName)

        member _.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            backgroundCompiler.NotifyProjectCleaned(options, userOpName)

        member _.ParseAndCheckFileInProject
            (
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults * FSharpCheckFileAnswer> =

            backgroundCompiler.ParseAndCheckFileInProject(fileName, fileVersion, sourceText, options, userOpName)

        member _.ParseAndCheckProject(options: FSharpProjectOptions, userOpName: string) : NodeCode<FSharpCheckProjectResults> =
            backgroundCompiler.ParseAndCheckProject(options, userOpName)

        member _.ParseFile
            (
                fileName: string,
                sourceText: ISourceText,
                options: FSharpParsingOptions,
                cache: bool,
                userOpName: string
            ) : Async<FSharpParseFileResults> =
            backgroundCompiler.ParseFile(fileName, sourceText, options, cache, userOpName)

        member _.ProjectChecked: IEvent<FSharpProjectOptions> = backgroundCompiler.ProjectChecked

        member _.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                sourceText: ISourceText option,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option =
            backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)
