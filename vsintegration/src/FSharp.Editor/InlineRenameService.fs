// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Editor.Implementation.InlineRename

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FailureInlineRenameInfo private () =
    interface IInlineRenameInfo with
        member __.CanRename = false
        member __.LocalizedErrorMessage = EditorFeaturesResources.You_cannot_rename_this_element
        member __.TriggerSpan = Unchecked.defaultof<_>
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = ""
        member __.FullDisplayName = ""
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName _replacementText = ""
        member __.GetReferenceEditSpan(_location, _cancellationToken) = Unchecked.defaultof<_>
        member __.GetConflictEditSpan(_location, _replacementText, _cancellationToken) = Nullable()
        member __.FindRenameLocationsAsync(_optionSet, _cancellationToken) = Task<IInlineRenameLocationSet>.FromResult null
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = false
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = false
    static member Instance = FailureInlineRenameInfo()

type internal DocumentLocations =
    { Document: Document
      Locations: InlineRenameLocation [] }

type internal InlineRenameLocationSet(locationsByDocument: DocumentLocations [], originalSolution: Solution) =
    interface IInlineRenameLocationSet with
        member __.Locations : IList<InlineRenameLocation> = 
            [| for doc in locationsByDocument do yield! doc.Locations |] :> _
        
        member this.GetReplacementsAsync(replacementText, _optionSet, cancellationToken) : Task<IInlineRenameReplacementInfo> =
            let rec applyChanges i (solution: Solution) =
                async {
                    if i = locationsByDocument.Length then 
                        return solution
                    else
                        let doc = locationsByDocument.[i]
                        let! oldSourceText = doc.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                        let changes = doc.Locations |> Seq.map (fun loc -> TextChange(loc.TextSpan, replacementText))
                        let newSource = oldSourceText.WithChanges(changes)
                        return! applyChanges (i + 1) (solution.WithDocumentText(doc.Document.Id, newSource))
                }
        
            async {
                let! newSolution = applyChanges 0 originalSolution
                return 
                    { new IInlineRenameReplacementInfo with
                        member __.NewSolution = newSolution
                        member __.ReplacementTextValid = true
                        member __.DocumentIds = locationsByDocument |> Seq.map (fun doc -> doc.Document.Id)
                        member __.GetReplacements(documentId) = Seq.empty }
            }
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        

type internal InlineRenameInfo
    (
        checker: FSharpChecker,
        options: FSharpProjectOptions, 
        document: Document,
        sourceText: SourceText, 
        symbolUse: FSharpSymbolUse,
        checkFileResults: FSharpCheckFileResults
    ) =

    let getDocumentText (document: Document) cancellationToken =
        match document.TryGetText() with
        | true, text -> text
        | _ -> document.GetTextAsync(cancellationToken).Result

    let triggerSpan =
        let span = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
        CommonHelpers.fixupSpan(sourceText, span)

    let underlyingGetSymbolUsesTaskLock = obj()
    let mutable underlyingGetSymbolUsesTask: Task<ImmutableDictionary<DocumentId, FSharpSymbolUse[]>> = null
    
    let getSymbolUses cancellationToken =
        lock underlyingGetSymbolUsesTaskLock <| fun _ ->
            if isNull underlyingGetSymbolUsesTask then
                // If this is the first call, then just start finding the initial set of rename locations.
                underlyingGetSymbolUsesTask <- 
                    async {
                        let! symbolUses =
                            if symbolUse.Symbol.IsPrivateToFile then
                                checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                            else
                                async {
                                    let! projectCheckResults = checker.ParseAndCheckProject(options)
                                    return! projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                                }
                        
                        return
                            (symbolUses 
                             |> Seq.collect (fun symbolUse -> 
                                  document.Project.Solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                             |> Seq.groupBy fst
                            ).ToImmutableDictionary(
                                (fun (id, _) -> id), 
                                fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
                    } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        
                underlyingGetSymbolUsesTask
            else
                // We already have a task to figure out the set of rename locations.
                // Let it finish, then ask it to get the rename locations with the updated options.
                underlyingGetSymbolUsesTask
        |> Async.AwaitTask

    interface IInlineRenameInfo with
        member __.CanRename = true
        member __.LocalizedErrorMessage = null
        member __.TriggerSpan = triggerSpan
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = symbolUse.Symbol.DisplayName
        member __.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName replacementText = replacementText

        member __.GetReferenceEditSpan(location, cancellationToken) =
            let text = getDocumentText location.Document cancellationToken
            CommonHelpers.fixupSpan(text, location.TextSpan)
        
        member __.GetConflictEditSpan(location, _replacementText, _cancellationToken) = Nullable(location.TextSpan)
        
        member __.FindRenameLocationsAsync(_optionSet, cancellationToken) =
            async {
                let! symbolUsesByDocumentId = getSymbolUses cancellationToken
                let! locationsByDocument =
                    symbolUsesByDocumentId
                    |> Seq.map (fun (KeyValue(documentId, symbolUses)) ->
                        async {
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            let locations =
                                symbolUses
                                |> Array.map (fun symbolUse ->
                                    let textSpan = CommonHelpers.fixupSpan(sourceText, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))
                                    InlineRenameLocation(document, textSpan))
                            return { Document = document; Locations = locations }
                        })
                    |> Async.Parallel
                return InlineRenameLocationSet(locationsByDocument, document.Project.Solution) :> IInlineRenameLocationSet
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true

[<ExportLanguageService(typeof<IEditorInlineRenameService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal InlineRenameService 
    [<ImportingConstructor>]
    (
        projectInfoManager: ProjectInfoManager,
        checkerProvider: FSharpCheckerProvider,
        [<ImportMany>] _refactorNotifyServices: seq<IRefactorNotifyService>
    ) =

    static member GetInlineRenameInfo(checker: FSharpChecker, document: Document, sourceText: SourceText, position: int, defines: string list, options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) : Async<IInlineRenameInfo> = 
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1 // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            
            match CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, cancellationToken) with 
            | Some (islandColumn, qualifiers, _) -> 
                let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(document.FilePath, textVersionHash, sourceText.ToString(), options)
                
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> return FailureInlineRenameInfo.Instance :> _
                | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 
        
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, islandColumn, textLine.Text.ToString(), qualifiers)
                
                match symbolUse with
                | Some symbolUse ->
                    match symbolUse.Symbol.DeclarationLocation with
                    | Some _ -> return InlineRenameInfo(checker, options, document, sourceText, symbolUse, checkFileResults) :> _
                    | _ -> return FailureInlineRenameInfo.Instance :> _
                | _ -> return FailureInlineRenameInfo.Instance :> _
            | None -> return FailureInlineRenameInfo.Instance :> _
        }
    
    interface IEditorInlineRenameService with
        member __.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<IInlineRenameInfo> =
            async {
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, document, sourceText, position, defines, options, textVersion.GetHashCode(), cancellationToken)
                | None -> return FailureInlineRenameInfo.Instance :> _
            }
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)