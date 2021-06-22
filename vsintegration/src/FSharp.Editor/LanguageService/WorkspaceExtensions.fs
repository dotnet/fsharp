[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.WorkspaceExtensions

open System
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis

[<AutoOpen>]
module private CheckerExtensions =

    type FSharpChecker with
        member checker.ParseDocument(document: Document, parsingOptions: FSharpParsingOptions, userOpName: string) =
            async {
                let! ct = Async.CancellationToken
                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask

                return! checker.ParseFile(document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions, userOpName=userOpName)
            }

        member checker.CheckDocument(document: Document, parseResults: FSharpParseFileResults, options: FSharpProjectOptions, userOpName: string) =
            async {
                let! ct = Async.CancellationToken

                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask
                let! textVersion = document.GetTextVersionAsync(ct) |> Async.AwaitTask

                let filePath = document.FilePath
                let textVersionHash = textVersion.GetHashCode()
            
                return! checker.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToFSharpSourceText(), options,userOpName=userOpName)
            }

        member checker.ParseAndCheckDocumentWithPossibleStaleResults(document: Document, options: FSharpProjectOptions, allowStaleResults: bool, userOpName: string) =
            async {
                let! ct = Async.CancellationToken

                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask
                let! textVersion = document.GetTextVersionAsync(ct) |> Async.AwaitTask

                let filePath = document.FilePath
                let textVersionHash = textVersion.GetHashCode()

                let parseAndCheckFile =
                    async {
                        let! (parseResults, checkFileAnswer) = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToFSharpSourceText(), options, userOpName=userOpName)
                        return
                            match checkFileAnswer with
                            | FSharpCheckFileAnswer.Aborted -> 
                                None
                            | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                                Some (parseResults, checkFileResults)
                    }

                let tryGetFreshResultsWithTimeout() =
                    async {
                        let! worker = Async.StartChild(async { try return! parseAndCheckFile with | _ -> return None }, millisecondsTimeout=document.Project.FSharpTimeUntilStaleCompletion)
                        try
                            return! worker
                        with :? TimeoutException ->
                            return None // worker is cancelled at this point, we cannot return it and wait its completion anymore
                    }

                let bindParsedInput(results: (FSharpParseFileResults * FSharpCheckFileResults) option) =
                    match results with
                    | Some(parseResults, checkResults) ->
                        Some (parseResults, parseResults.ParseTree, checkResults)
                    | None -> None

                if allowStaleResults then
                    let! freshResults = tryGetFreshResultsWithTimeout()
                    
                    let! results =
                        match freshResults with
                        | Some x -> async.Return (Some x)
                        | None ->
                            async {
                                match checker.TryGetRecentCheckResultsForFile(filePath, options, userOpName=userOpName) with
                                | Some (parseResults, checkFileResults, _) ->
                                    return Some (parseResults, checkFileResults)
                                | None ->
                                    return! parseAndCheckFile
                            }
                    return bindParsedInput results
                else 
                    let! results = parseAndCheckFile
                    return bindParsedInput results
            }

        member checker.ParseAndCheckDocument(document: Document, options: FSharpProjectOptions, userOpName: string, ?allowStaleResults: bool) =
            async {
                let allowStaleResults =
                    match allowStaleResults with 
                    | Some b -> b
                    | _ ->  document.Project.IsFSharpStaleCompletionResultsEnabled
                return! checker.ParseAndCheckDocumentWithPossibleStaleResults(document, options, allowStaleResults, userOpName=userOpName)
            }

[<RequireQualifiedAccess>]
module private ProjectCache =

    let Projects = ConditionalWeakTable<Project, FSharpChecker * FSharpProjectOptionsManager * FSharpParsingOptions * FSharpProjectOptions>()    

type Solution with

    member private this.GetFSharpService() =
        this.Workspace.Services.GetRequiredService<IFSharpWorkspaceService>()

type Project with

    member this.GetFSharpProjectOptionsAsync() =
        async {
            if this.IsFSharp then
                match ProjectCache.Projects.TryGetValue(this) with
                | true, result -> return result
                | _ ->
                    let service = this.Solution.GetFSharpService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager
                    let! ct = Async.CancellationToken
                    match! projectOptionsManager.TryGetOptionsByProject(this, ct) with
                    | None -> return raise(System.OperationCanceledException("FSharp project options not found."))
                    | Some(parsingOptions, projectOptions) ->
                        let result = (service.Checker, projectOptionsManager, parsingOptions, projectOptions)
                        ProjectCache.Projects.Add(this, result)
                        return result
            else
                return raise(System.OperationCanceledException("Project is not a FSharp project."))
        }

    member this.GetFSharpProjectDefinesAsync() =
        async {
            let! _, _, parsingOptions, _ = this.GetFSharpProjectOptionsAsync()
            return CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
        }

type Document with

    member this.GetFSharpSyntaxDefines() =
        if this.Project.IsFSharp then
            let service = this.Project.Solution.GetFSharpService()
            service.FSharpProjectOptionsManager.GetCompilationDefinesForEditingDocument(this)
        else
            []
    
    member this.GetFSharpParseResultsAsync() =
        async {
            let! checker, _, parsingOptions, _ = this.Project.GetFSharpProjectOptionsAsync()
            return! checker.ParseDocument(this, parsingOptions, nameof(this.GetFSharpParseResultsAsync))
        }

    member this.GetFSharpParseAndCheckResultsAsync() =
        async {
            let! checker, _, _, projectOptions = this.Project.GetFSharpProjectOptionsAsync()
            match! checker.ParseAndCheckDocument(this, projectOptions, nameof(this.GetFSharpParseAndCheckResultsAsync), allowStaleResults = false) with
            | Some(parseResults, _, checkResults) ->
                return (parseResults, checkResults)
            | _ ->
                return raise(System.OperationCanceledException("Unable to get FSharp parse and check results."))
        }

    member this.GetFSharpSemanticClassificationAsync() =
        async {
            let! checker, _, _, projectOptions = this.Project.GetFSharpProjectOptionsAsync()
            match! checker.GetBackgroundSemanticClassificationForFile(this.FilePath, projectOptions) with
            | Some results -> return results
            | _ -> return raise(System.OperationCanceledException("Unable to get FSharp semantic classification."))
        }

    member this.FindFSharpReferencesAsync(symbol, onFound) =
        async {
            let! checker, _, _, projectOptions = this.Project.GetFSharpProjectOptionsAsync()
            let! symbolUses = checker.FindBackgroundReferencesInFile(this.FilePath, projectOptions, symbol, canInvalidateProject = false)
            let! ct = Async.CancellationToken
            let! sourceText = this.GetTextAsync ct |> Async.AwaitTask
            for symbolUse in symbolUses do 
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                | Some textSpan ->
                    do! onFound textSpan symbolUse
                | _ ->
                    ()
        }

    member this.TryFindFSharpLexerSymbolAsync(position, lookupKind, wholeActivePattern, allowStringToken) =
        async {
            let! defines = this.Project.GetFSharpProjectDefinesAsync()
            let! ct = Async.CancellationToken
            let! sourceText = this.GetTextAsync(ct) |> Async.AwaitTask
            return Tokenizer.getSymbolAtPosition(this.Id, sourceText, position, this.FilePath, defines, lookupKind, wholeActivePattern, allowStringToken)
        }

type Project with

    member this.FindFSharpReferencesAsync(symbol, onFound) =
        async {
            for doc in this.Documents do
                do! doc.FindFSharpReferencesAsync(symbol, fun textSpan range -> onFound doc textSpan range)
        }
