[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.WorkspaceExtensions

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols

[<AutoOpen>]
module private CheckerExtensions =

    let getProjectSnapshot (document: Document, options: FSharpProjectOptions) =
        async {
            let project = document.Project
            let solution = project.Solution
            // TODO cache?
            let projects =
                solution.Projects
                |> Seq.map (fun p -> p.FilePath, p.Documents |> Seq.map (fun d -> d.FilePath, d) |> Map)
                |> Map

            let getFileSnapshot (options: FSharpProjectOptions) path =
                async {
                    let project = projects[options.ProjectFileName]
                    let document = project[path]
                    let! version = document.GetTextVersionAsync() |> Async.AwaitTask

                    let getSource () =
                        task {
                            let! sourceText = document.GetTextAsync()
                            return sourceText.ToFSharpSourceText()
                        }

                    return
                        {
                            FileName = path
                            Version = version.ToString()
                            GetSource = getSource
                        }
                }

            return! FSharpProjectSnapshot.FromOptions(options, getFileSnapshot)
        }

    type FSharpChecker with

        /// Parse the source text from the Roslyn document.
        member checker.ParseDocument(document: Document, parsingOptions: FSharpParsingOptions, userOpName: string) =
            async {
                let! ct = Async.CancellationToken
                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask

                return! checker.ParseFile(document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions, userOpName = userOpName)
            }

        member checker.ParseDocumentUsingTransparentCompiler(document: Document, options: FSharpProjectOptions, userOpName: string) =
            async {
                let! projectSnapshot = getProjectSnapshot (document, options)
                return! checker.ParseFile(document.FilePath, projectSnapshot, userOpName = userOpName)
            }

        member checker.ParseAndCheckDocumentUsingTransparentCompiler
            (
                document: Document,
                options: FSharpProjectOptions,
                userOpName: string
            ) =
            async {
                let! projectSnapshot = getProjectSnapshot (document, options)

                let! (parseResults, checkFileAnswer) = checker.ParseAndCheckFileInProject(document.FilePath, projectSnapshot, userOpName)

                return
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Aborted -> None
                    | FSharpCheckFileAnswer.Succeeded (checkFileResults) -> Some(parseResults, checkFileResults)
            }

        /// Parse and check the source text from the Roslyn document with possible stale results.
        member checker.ParseAndCheckDocumentWithPossibleStaleResults
            (
                document: Document,
                options: FSharpProjectOptions,
                allowStaleResults: bool,
                userOpName: string
            ) =
            async {
                let! ct = Async.CancellationToken

                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask
                let! textVersion = document.GetTextVersionAsync(ct) |> Async.AwaitTask

                let filePath = document.FilePath
                let textVersionHash = textVersion.GetHashCode()

                let parseAndCheckFile =
                    async {
                        let! (parseResults, checkFileAnswer) =
                            checker.ParseAndCheckFileInProject(
                                filePath,
                                textVersionHash,
                                sourceText.ToFSharpSourceText(),
                                options,
                                userOpName = userOpName
                            )

                        return
                            match checkFileAnswer with
                            | FSharpCheckFileAnswer.Aborted -> None
                            | FSharpCheckFileAnswer.Succeeded (checkFileResults) -> Some(parseResults, checkFileResults)
                    }

                let tryGetFreshResultsWithTimeout () =
                    async {
                        let! worker =
                            Async.StartChild(
                                async {
                                    try
                                        return! parseAndCheckFile
                                    with _ ->
                                        return None
                                },
                                millisecondsTimeout = document.Project.FSharpTimeUntilStaleCompletion
                            )

                        try
                            return! worker
                        with :? TimeoutException ->
                            return None // worker is cancelled at this point, we cannot return it and wait its completion anymore
                    }

                if allowStaleResults then
                    let! freshResults = tryGetFreshResultsWithTimeout ()

                    let! results =
                        match freshResults with
                        | Some x -> async.Return(Some x)
                        | None ->
                            async {
                                match checker.TryGetRecentCheckResultsForFile(filePath, options, userOpName = userOpName) with
                                | Some (parseResults, checkFileResults, _) -> return Some(parseResults, checkFileResults)
                                | None -> return! parseAndCheckFile
                            }

                    return results
                else
                    let! results = parseAndCheckFile
                    return results
            }

        /// Parse and check the source text from the Roslyn document.
        member checker.ParseAndCheckDocument
            (
                document: Document,
                options: FSharpProjectOptions,
                userOpName: string,
                ?allowStaleResults: bool
            ) =
            async {

                if document.Project.UseTransparentCompiler then
                    return! checker.ParseAndCheckDocumentUsingTransparentCompiler(document, options, userOpName)
                else
                    let allowStaleResults =
                        match allowStaleResults with
                        | Some b -> b
                        | _ -> document.Project.IsFSharpStaleCompletionResultsEnabled

                    return!
                        checker.ParseAndCheckDocumentWithPossibleStaleResults(document, options, allowStaleResults, userOpName = userOpName)
            }

[<RequireQualifiedAccess>]
module internal ProjectCache =

    /// This is a cache to maintain FSharpParsingOptions and FSharpProjectOptions per Roslyn Project.
    /// The Roslyn Project is held weakly meaning when it is cleaned up by the GC, the FSharParsingOptions and FSharpProjectOptions will be cleaned up by the GC.
    /// At some point, this will be the main caching mechanism for FCS projects instead of FCS itself.
    let Projects =
        ConditionalWeakTable<Project, FSharpChecker * FSharpProjectOptionsManager * FSharpParsingOptions * FSharpProjectOptions>()

type Solution with

    /// Get the instance of IFSharpWorkspaceService.
    member internal this.GetFSharpWorkspaceService() =
        this.Workspace.Services.GetRequiredService<IFSharpWorkspaceService>()

type Document with

    /// Get the FSharpParsingOptions and FSharpProjectOptions from the F# project that is associated with the given F# document.
    member this.GetFSharpCompilationOptionsAsync(userOpName) =
        async {
            if this.Project.IsFSharp then
                match ProjectCache.Projects.TryGetValue(this.Project) with
                | true, result -> return result
                | _ ->
                    let service = this.Project.Solution.GetFSharpWorkspaceService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager
                    let! ct = Async.CancellationToken

                    match! projectOptionsManager.TryGetOptionsForDocumentOrProject(this, ct, userOpName) with
                    | None -> return raise (System.OperationCanceledException("FSharp project options not found."))
                    | Some (parsingOptions, projectOptions) ->
                        let result =
                            (service.Checker, projectOptionsManager, parsingOptions, projectOptions)

                        return
                            ProjectCache.Projects.GetValue(
                                this.Project,
                                Runtime.CompilerServices.ConditionalWeakTable<_, _>.CreateValueCallback (fun _ -> result)
                            )
            else
                return raise (System.OperationCanceledException("Document is not a FSharp document."))
        }

    /// Get the compilation defines from F# project that is associated with the given F# document.
    member this.GetFSharpCompilationDefinesAsync(userOpName) =
        async {
            let! _, _, parsingOptions, _ = this.GetFSharpCompilationOptionsAsync(userOpName)
            return CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
        }

    /// Get the compilation defines and language version from F# project that is associated with the given F# document.
    member this.GetFSharpCompilationDefinesAndLangVersionAsync(userOpName) =
        async {
            let! _, _, parsingOptions, _ = this.GetFSharpCompilationOptionsAsync(userOpName)
            return CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions, parsingOptions.LangVersionText
        }

    /// Get the instance of the FSharpChecker from the workspace by the given F# document.
    member this.GetFSharpChecker() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.Checker

    /// Get the instance of the FSharpMetadataAsSourceService from the workspace by the given F# document.
    member this.GetFSharpMetadataAsSource() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.MetadataAsSource

    /// A non-async call that quickly gets FSharpParsingOptions of the given F# document.
    /// This tries to get the FSharpParsingOptions by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the FSharpParsingOptions.
    member this.GetFSharpQuickParsingOptions() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.TryGetQuickParsingOptionsForEditingDocumentOrProject(this.Id, this.FilePath)

    /// A non-async call that quickly gets the defines and F# language version of the given F# document.
    /// This tries to get the data by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the defines and the language version.
    member this.GetFSharpQuickDefinesAndLangVersion() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.GetCompilationDefinesAndLangVersionForEditingDocument(this)

    /// A non-async call that quickly gets the defines of the given F# document.
    /// This tries to get the defines by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the defines.
    member this.GetFSharpQuickDefines() =
        this.GetFSharpQuickDefinesAndLangVersion() |> fst

    /// Parses the given F# document.
    member this.GetFSharpParseResultsAsync(userOpName) =
        async {
            let! checker, _, parsingOptions, options = this.GetFSharpCompilationOptionsAsync(userOpName)

            if this.Project.UseTransparentCompiler then
                return! checker.ParseDocumentUsingTransparentCompiler(this, options, userOpName)
            else
                return! checker.ParseDocument(this, parsingOptions, userOpName)
        }

    /// Parses and checks the given F# document.
    member this.GetFSharpParseAndCheckResultsAsync(userOpName) =
        async {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            match! checker.ParseAndCheckDocument(this, projectOptions, userOpName, allowStaleResults = false) with
            | Some results -> return results
            | _ -> return raise (System.OperationCanceledException("Unable to get FSharp parse and check results."))
        }

    /// Get the semantic classifications of the given F# document.
    member this.GetFSharpSemanticClassificationAsync(userOpName) =
        async {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            let! result =
                if this.Project.UseTransparentCompiler then
                    async {
                        let! projectSnapshot = getProjectSnapshot (this, projectOptions)
                        return! checker.GetBackgroundSemanticClassificationForFile(this.FilePath, projectSnapshot)
                    }
                else
                    checker.GetBackgroundSemanticClassificationForFile(this.FilePath, projectOptions)

            return
                result
                |> Option.defaultWith (fun _ -> raise (System.OperationCanceledException("Unable to get FSharp semantic classification.")))
        }

    /// Find F# references in the given F# document.
    member this.FindFSharpReferencesAsync(symbol, onFound, userOpName) =
        async {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            let! symbolUses =

                if this.Project.UseTransparentCompiler then
                    async {
                        let! projectSnapshot = getProjectSnapshot (this, projectOptions)
                        return! checker.FindBackgroundReferencesInFile(this.FilePath, projectSnapshot, symbol)
                    }
                else
                    checker.FindBackgroundReferencesInFile(
                        this.FilePath,
                        projectOptions,
                        symbol,
                        canInvalidateProject = false,
                        fastCheck = this.Project.IsFastFindReferencesEnabled
                    )

            for symbolUse in symbolUses do
                do! onFound symbolUse
        }

    /// Try to find a F# lexer/token symbol of the given F# document and position.
    member this.TryFindFSharpLexerSymbolAsync(position, lookupKind, wholeActivePattern, allowStringToken, userOpName) =
        async {
            let! defines, langVersion = this.GetFSharpCompilationDefinesAndLangVersionAsync(userOpName)
            let! ct = Async.CancellationToken
            let! sourceText = this.GetTextAsync(ct) |> Async.AwaitTask

            return
                Tokenizer.getSymbolAtPosition (
                    this.Id,
                    sourceText,
                    position,
                    this.FilePath,
                    defines,
                    lookupKind,
                    wholeActivePattern,
                    allowStringToken,
                    Some langVersion,
                    ct
                )
        }

type Project with

    /// Find F# references in the given project.
    member this.FindFSharpReferencesAsync(symbol: FSharpSymbol, onFound, userOpName, ct) : Task =
        backgroundTask {

            let declarationLocation =
                symbol.SignatureLocation
                |> Option.map Some
                |> Option.defaultValue symbol.DeclarationLocation

            let declarationDocument =
                declarationLocation |> Option.bind this.Solution.TryGetDocumentFromFSharpRange

            let! canSkipDocuments =
                match declarationDocument with
                | Some document when this.IsFastFindReferencesEnabled && document.Project = this ->
                    backgroundTask {
                        let! _, _, _, options =
                            document.GetFSharpCompilationOptionsAsync(userOpName)
                            |> RoslynHelpers.StartAsyncAsTask ct

                        let signatureFile =
                            if not (document.FilePath |> isSignatureFile) then
                                $"{document.FilePath}i"
                            else
                                null

                        return
                            options.SourceFiles
                            |> Seq.takeWhile ((<>) document.FilePath)
                            |> Seq.filter ((<>) signatureFile)
                            |> Set
                    }
                | _ -> Task.FromResult Set.empty

            let documents =
                this.Documents
                |> Seq.filter (fun document -> not (canSkipDocuments.Contains document.FilePath))

            if this.IsFastFindReferencesEnabled then
                do!
                    documents
                    |> Seq.map (fun doc ->
                        Task.Run(fun () ->
                            doc.FindFSharpReferencesAsync(symbol, (fun range -> onFound doc range), userOpName)
                            |> RoslynHelpers.StartAsyncUnitAsTask ct))
                    |> Task.WhenAll
            else
                for doc in documents do
                    do!
                        doc.FindFSharpReferencesAsync(symbol, (fun range -> onFound doc range), userOpName)
                        |> RoslynHelpers.StartAsyncAsTask ct
        }

    member this.GetFSharpCompilationOptionsAsync(ct: CancellationToken) =
        backgroundTask {
            if this.IsFSharp then
                match ProjectCache.Projects.TryGetValue(this) with
                | true, result -> return result
                | _ ->
                    let service = this.Solution.GetFSharpWorkspaceService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager

                    match! projectOptionsManager.TryGetOptionsByProject(this, ct) with
                    | None -> return raise (OperationCanceledException("FSharp project options not found."))
                    | Some (parsingOptions, projectOptions) ->
                        let result =
                            (service.Checker, projectOptionsManager, parsingOptions, projectOptions)

                        return ProjectCache.Projects.GetValue(this, ConditionalWeakTable<_, _>.CreateValueCallback (fun _ -> result))
            else
                return raise (OperationCanceledException("Project is not a FSharp project."))
        }
