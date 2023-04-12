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


    let ComputeParseFile (file: FSharpFile) (projectSnapshot: FSharpProjectSnapshot) userOpName _key = node {

        return ()

    }


    let ComputeParseAndCheckFileInProject (fileName: string) (projectSnapshot: FSharpProjectSnapshot) _key =
        node {

            let! bootstrapInfoOpt, creationDiags = getBootstrapInfo projectSnapshot // probably cache

            match bootstrapInfoOpt with
            | None ->
                let parseTree = EmptyParsedInput(fileName, (false, false))
                let parseResults = FSharpParseFileResults(creationDiags, parseTree, true, [||])
                return (parseResults, FSharpCheckFileAnswer.Aborted)

            | Some bootstrapInfo ->


                

                // Do the parsing.
                let parsingOptions =
                    FSharpParsingOptions.FromTcConfig(
                        bootstrapInfo.TcConfig,
                        projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName) |> Array.ofSeq,
                        projectSnapshot.UseScriptResolutionRules
                    )

                // TODO: what is this?
                // GraphNode.SetPreferredUILang tcPrior.TcConfig.preferredUiLang

                let parseDiagnostics, parseTree, anyErrors =
                    ParseAndCheckFile.parseFile (
                        sourceText,
                        fileName,
                        parsingOptions,
                        userOpName,
                        suggestNamesForErrors,
                        captureIdentifiersWhenParsing
                    )

                // TODO: check if we need this in parse results
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
