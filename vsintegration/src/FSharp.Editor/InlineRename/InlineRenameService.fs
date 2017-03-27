// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler
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
    static member Instance = FailureInlineRenameInfo() :> IInlineRenameInfo

type internal DocumentLocations =
    { Document: Document
      Locations: InlineRenameLocation [] }

type internal InlineRenameLocationSet(locationsByDocument: DocumentLocations [], originalSolution: Solution, symbolKind: LexerSymbolKind, symbol: FSharpSymbol) =
    interface IInlineRenameLocationSet with
        member __.Locations : IList<InlineRenameLocation> =
            upcast [| for doc in locationsByDocument do yield! doc.Locations |].ToList()
        
        member this.GetReplacementsAsync(replacementText, _optionSet, cancellationToken) : Task<IInlineRenameReplacementInfo> =
            let rec applyChanges i (solution: Solution) =
                async {
                    if i = locationsByDocument.Length then 
                        return solution
                    else
                        let doc = locationsByDocument.[i]
                        let! oldSourceText = doc.Document.GetTextAsync(cancellationToken)
                        let changes = doc.Locations |> Seq.map (fun loc -> TextChange(loc.TextSpan, replacementText))
                        let newSource = oldSourceText.WithChanges(changes)
                        return! applyChanges (i + 1) (solution.WithDocumentText(doc.Document.Id, newSource))
                }
        
            async {
                let! newSolution = applyChanges 0 originalSolution
                return 
                    { new IInlineRenameReplacementInfo with
                        member __.NewSolution = newSolution
                        member __.ReplacementTextValid = CommonHelpers.isValidNameForSymbol(symbolKind, symbol, replacementText)
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
        lexerSymbol: LexerSymbol,
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
        SymbolHelpers.getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, projectInfoManager, checker, document.Project.Solution)
        |> Async.cache

    interface IInlineRenameInfo with
        member __.CanRename = true
        member __.LocalizedErrorMessage = null
        member __.TriggerSpan = triggerSpan
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = symbolUse.Symbol.DisplayName
        member __.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName replacementText = Lexhelp.Keywords.NormalizeIdentifierBackticks replacementText

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
                            let! sourceText = document.GetTextAsync(cancellationToken)
                            let locations =
                                symbolUses
                                |> Array.map (fun symbolUse ->
                                    let textSpan = CommonHelpers.fixupSpan(sourceText, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))
                                    InlineRenameLocation(document, textSpan))
                            return { Document = document; Locations = locations }
                        })
                    |> Async.Parallel
                return InlineRenameLocationSet(locationsByDocument, document.Project.Solution, lexerSymbol.Kind, symbolUse.Symbol) :> IInlineRenameLocationSet
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
                                      defines: string list, options: FSharpProjectOptions) : Async<IInlineRenameInfo option> = 
        asyncMaybe {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! symbol = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland)
            let! declLoc = symbolUse.GetDeclarationLocation(document)
            return InlineRenameInfo(checker, projectInfoManager, document, sourceText, symbol, symbolUse, declLoc, checkFileResults) :> IInlineRenameInfo
        }
    
    interface IEditorInlineRenameService with
        member __.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<IInlineRenameInfo> =
            asyncMaybe {
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, projectInfoManager, document, sourceText, position, defines, options)
            }
            |> Async.map (Option.defaultValue FailureInlineRenameInfo.Instance)
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
