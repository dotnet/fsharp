// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor

open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open Symbols

type internal FailureInlineRenameInfo private () =
    interface IFSharpInlineRenameInfo with
        member __.CanRename = false
        member __.LocalizedErrorMessage = FSharpEditorFeaturesResources.You_cannot_rename_this_element
        member __.TriggerSpan = Unchecked.defaultof<_>
        member __.HasOverloads = false
        member __.ForceRenameOverloads = true
        member __.DisplayName = ""
        member __.FullDisplayName = ""
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName _ = ""
        member __.GetReferenceEditSpan(_, _) = Unchecked.defaultof<_>
        member __.GetConflictEditSpan(_, _, _) = Nullable()
        member __.FindRenameLocationsAsync(_, _) = Task<IFSharpInlineRenameLocationSet>.FromResult null
        member __.TryOnBeforeGlobalSymbolRenamed(_, _, _) = false
        member __.TryOnAfterGlobalSymbolRenamed(_, _, _) = false
    static member Instance = FailureInlineRenameInfo() :> IFSharpInlineRenameInfo

type internal InlineRenameLocationSet(locations: FSharpInlineRenameLocation [], originalSolution: Solution, symbolKind: LexerSymbolKind, symbol: FSharpSymbol) =
    interface IFSharpInlineRenameLocationSet with
        member __.Locations = upcast locations.ToList()
        
        member __.GetReplacementsAsync(replacementText, _optionSet, cancellationToken) : Task<IFSharpInlineRenameReplacementInfo> =
            let rec applyChanges (solution: Solution) (locationsByDocument: (Document * FSharpInlineRenameLocation list) list) =
                async {
                    match locationsByDocument with
                    | [] -> return solution
                    | (document, locations) :: rest ->
                        let! oldSource = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                        let newSource = oldSource.WithChanges(locations |> List.map (fun l -> TextChange(l.TextSpan, replacementText)))
                        return! applyChanges (solution.WithDocumentText(document.Id, newSource)) rest
                }
        
            async {
                let! newSolution = applyChanges originalSolution (locations |> Array.toList |> List.groupBy (fun x -> x.Document))
                let replacementText =
                    match symbolKind with
                    | LexerSymbolKind.GenericTypeParameter
                    | LexerSymbolKind.StaticallyResolvedTypeParameter -> replacementText
                    | _ -> Keywords.NormalizeIdentifierBackticks replacementText
                return 
                    { new IFSharpInlineRenameReplacementInfo with
                        member __.NewSolution = newSolution
                        member __.ReplacementTextValid = Tokenizer.isValidNameForSymbol(symbolKind, symbol, replacementText)
                        member __.DocumentIds = locations |> Seq.map (fun doc -> doc.Document.Id) |> Seq.distinct
                        member __.GetReplacements _ = Seq.empty }
            }
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)

type internal InlineRenameInfo
    (
        checker: FSharpChecker,
        projectInfoManager: FSharpProjectOptionsManager,
        document: Document,
        triggerSpan: TextSpan, 
        lexerSymbol: LexerSymbol,
        symbolUse: FSharpSymbolUse,
        declLoc: SymbolDeclarationLocation,
        checkFileResults: FSharpCheckFileResults
    ) =

    static let userOpName = "InlineRename"

    let getDocumentText (document: Document) cancellationToken =
        match document.TryGetText() with
        | true, text -> text
        | _ -> document.GetTextAsync(cancellationToken).Result

    let symbolUses = 
        SymbolHelpers.getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, projectInfoManager, checker, document.Project.Solution, userOpName)
        |> Async.cache

    interface IFSharpInlineRenameInfo with
        member __.CanRename = true
        member __.LocalizedErrorMessage = null
        member __.TriggerSpan = triggerSpan
        member __.HasOverloads = false
        member __.ForceRenameOverloads = false
        member __.DisplayName = symbolUse.Symbol.DisplayName
        member __.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
        member __.Glyph = Glyph.MethodPublic
        member __.GetFinalSymbolName replacementText = replacementText

        member __.GetReferenceEditSpan(location, cancellationToken) =
            let text = getDocumentText location.Document cancellationToken
            Tokenizer.fixupSpan(text, location.TextSpan)
        
        member __.GetConflictEditSpan(location, replacementText, cancellationToken) = 
            let text = getDocumentText location.Document cancellationToken
            let spanText = text.ToString(location.TextSpan)
            let position = spanText.LastIndexOf(replacementText, StringComparison.Ordinal)
            if position < 0 then Nullable()
            else Nullable(TextSpan(location.TextSpan.Start + position, replacementText.Length))
        
        member __.FindRenameLocationsAsync(_optionSet, cancellationToken) =
            async {
                let! symbolUsesByDocumentId = symbolUses
                let! locations =
                    symbolUsesByDocumentId
                    |> Seq.map (fun (KeyValue(documentId, symbolUses)) ->
                        async {
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            return 
                                [| for symbolUse in symbolUses do
                                     match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                                     | Some span ->
                                         let textSpan = Tokenizer.fixupSpan(sourceText, span)
                                         yield FSharpInlineRenameLocation(document, textSpan) 
                                     | None -> () |]
                        })
                    |> Async.Parallel
                    |> Async.map Array.concat

                return InlineRenameLocationSet(locations, document.Project.Solution, lexerSymbol.Kind, symbolUse.Symbol) :> IFSharpInlineRenameLocationSet
            } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)
        
        member __.TryOnBeforeGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true
        member __.TryOnAfterGlobalSymbolRenamed(_workspace, _changedDocumentIDs, _replacementText) = true

[<Export(typeof<IFSharpEditorInlineRenameService>); Shared>]
type internal InlineRenameService 
    [<ImportingConstructor>]
    (
        projectInfoManager: FSharpProjectOptionsManager,
        checkerProvider: FSharpCheckerProvider
    ) =

    static let userOpName = "InlineRename"
    static member GetInlineRenameInfo(checker: FSharpChecker, projectInfoManager: FSharpProjectOptionsManager, document: Document, sourceText: SourceText, position: int, 
                                      defines: string list, options: FSharpProjectOptions) : Async<IFSharpInlineRenameInfo option> = 
        asyncMaybe {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, userOpName = userOpName)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland)
            let! declLoc = symbolUse.GetDeclarationLocation(document)

            let! span = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
            let triggerSpan = Tokenizer.fixupSpan(sourceText, span)

            return InlineRenameInfo(checker, projectInfoManager, document, triggerSpan, symbol, symbolUse, declLoc, checkFileResults) :> IFSharpInlineRenameInfo
        }
    
    interface IFSharpEditorInlineRenameService with
        member __.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<IFSharpInlineRenameInfo> =
            asyncMaybe {
                let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
                return! InlineRenameService.GetInlineRenameInfo(checkerProvider.Checker, projectInfoManager, document, sourceText, position, defines, projectOptions)
            }
            |> Async.map (Option.defaultValue FailureInlineRenameInfo.Instance)
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)
