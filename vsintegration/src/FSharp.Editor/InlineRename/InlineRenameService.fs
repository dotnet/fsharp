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
        projectInfoManager: ProjectInfoManager,
        document: Document,
        sourceText: SourceText, 
        symbolUse: FSharpSymbolUse,
        declLoc: SymbolDeclarationLocation,
        checkFileResults: FSharpCheckFileResults
    ) =

    let getDocumentText (document: Document) cancellationToken =
        match document.TryGetText() with
        | true, text -> text
        | _ -> document.GetTextAsync(cancellationToken).Result

    let triggerSpan =
        let span = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
        CommonHelpers.fixupSpan(sourceText, span)

    let symbolUses =
        async {
            let! symbolUses =
                match declLoc with
                | SymbolDeclarationLocation.CurrentDocument ->
                    checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                | SymbolDeclarationLocation.Projects (projects, isInternalToProject) -> 
                    let projects =
                        if isInternalToProject then projects
                        else 
                            [ for project in projects do
                                yield project
                                yield! project.GetDependentProjects() ]
                            |> List.distinctBy (fun x -> x.Id)

                    projects
                    |> Seq.map (fun project ->
                        async {
                            match projectInfoManager.TryGetOptionsForProject(project.Id) with
                            | Some options ->
                                let! projectCheckResults = checker.ParseAndCheckProject(options)
                                return! projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                            | None -> return [||]
                        })
                    |> Async.Parallel
                    |> Async.Map Array.concat
            
            return
                (symbolUses 
                 |> Seq.collect (fun symbolUse -> 
                      document.Project.Solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                 |> Seq.groupBy fst
                ).ToImmutableDictionary(
                    (fun (id, _) -> id), 
                    fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
        } |> Async.Cache

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
                let! symbolUsesByDocumentId = symbolUses
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

    static member GetInlineRenameInfo(checker: FSharpChecker, projectInfoManager: ProjectInfoManager, document: Document, sourceText: SourceText, position: int, 
                                      defines: string list, options: FSharpProjectOptions, textVersionHash: int) : Async<IInlineRenameInfo> = 
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1 // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            
            match CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Fuzzy) with 
            | Some symbol -> 
                let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(document.FilePath, textVersionHash, sourceText.ToString(), options)
                
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> return FailureInlineRenameInfo.Instance :> _
                | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 
        
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, textLine.Text.ToString(), [symbol.Text])
                
                match symbolUse with
                | Some symbolUse ->
                    match symbolUse.GetDeclarationLocation(document) with
                    | Some declLoc -> return InlineRenameInfo(checker, projectInfoManager, document, sourceText, symbolUse, declLoc, checkFileResults) :> _
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
                    return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, projectInfoManager, document, sourceText, position, defines, options, hash textVersion)
                | None -> return FailureInlineRenameInfo.Instance :> _
            }
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)