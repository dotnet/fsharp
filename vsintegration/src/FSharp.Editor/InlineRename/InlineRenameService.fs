// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Composition
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open Symbols

type internal InlineRenameReplacementInfo(newSolution: Solution, replacementTextValid: bool, documentIds: IEnumerable<DocumentId>) =
    inherit FSharpInlineRenameReplacementInfo()

    override _.NewSolution = newSolution
    override _.ReplacementTextValid = replacementTextValid
    override _.DocumentIds = documentIds
    override _.GetReplacements _ = Seq.empty

type internal InlineRenameLocationSet(locations: FSharpInlineRenameLocation [], originalSolution: Solution, symbolKind: LexerSymbolKind, symbol: FSharpSymbol) =
    inherit FSharpInlineRenameLocationSet()

    override _.Locations = upcast locations.ToList()
        
    override _.GetReplacementsAsync(replacementText, cancellationToken) : Task<FSharpInlineRenameReplacementInfo> =
        let rec applyChanges (solution: Solution) (locationsByDocument: (Document * FSharpInlineRenameLocation list) list) =
            backgroundTask {
                match locationsByDocument with
                | [] -> return solution
                | (document, locations) :: rest ->
                    let! oldSource = document.GetTextAsync(cancellationToken)
                    let newSource = oldSource.WithChanges(locations |> List.map (fun l -> TextChange(l.TextSpan, replacementText)))
                    return! applyChanges (solution.WithDocumentText(document.Id, newSource)) rest
            }
        
        backgroundTask {
            let! newSolution = applyChanges originalSolution (locations |> Array.toList |> List.groupBy (fun x -> x.Document))
            let replacementText =
                match symbolKind with
                | LexerSymbolKind.GenericTypeParameter
                | LexerSymbolKind.StaticallyResolvedTypeParameter -> replacementText
                | _ -> FSharpKeywords.NormalizeIdentifierBackticks replacementText
            let replacementTextValid = Tokenizer.isValidNameForSymbol(symbolKind, symbol, replacementText)
            let documentIds = locations |> Seq.map (fun doc -> doc.Document.Id) |> Seq.distinct
            return new InlineRenameReplacementInfo(newSolution, replacementTextValid, documentIds) :> FSharpInlineRenameReplacementInfo
        }

type internal InlineRenameInfo
    (
        document: Document,
        triggerSpan: TextSpan, 
        lexerSymbol: LexerSymbol,
        symbolUse: FSharpSymbolUse,
        declLoc: SymbolDeclarationLocation,
        checkFileResults: FSharpCheckFileResults
    ) =

    inherit FSharpInlineRenameInfo()

    let getDocumentText (document: Document) cancellationToken =
        match document.TryGetText() with
        | true, text -> text
        | _ -> document.GetTextAsync(cancellationToken).Result

    let symbolUses ct =
        SymbolHelpers.getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, document.Project.Solution, ct)

    override _.CanRename = true
    override _.LocalizedErrorMessage = null
    override _.TriggerSpan = triggerSpan
    override _.HasOverloads = false
    override _.ForceRenameOverloads = false
    override _.DisplayName = symbolUse.Symbol.DisplayName
    override _.FullDisplayName = try symbolUse.Symbol.FullName with _ -> symbolUse.Symbol.DisplayName
    override _.Glyph = Glyph.MethodPublic
    override _.GetFinalSymbolName replacementText = replacementText
    override _.DefinitionLocations = ImmutableArray.Create(new FSharpInlineRenameLocation(document, triggerSpan))

    override _.GetReferenceEditSpan(location, cancellationToken) =
        let text = getDocumentText location.Document cancellationToken
        Tokenizer.fixupSpan(text, location.TextSpan)
        
    override _.GetConflictEditSpan(location, replacementText, cancellationToken) = 
        let text = getDocumentText location.Document cancellationToken
        let spanText = text.ToString(location.TextSpan)
        let position = spanText.LastIndexOf(replacementText, StringComparison.Ordinal)
        if position < 0 then Nullable()
        else Nullable(TextSpan(location.TextSpan.Start + position, replacementText.Length))
        
    override _.FindRenameLocationsAsync(_, _, cancellationToken) =
        backgroundTask {
            let! symbolUsesByDocumentId = symbolUses cancellationToken
            let! locations =
                symbolUsesByDocumentId
                |> Seq.map (fun (KeyValue(documentId, symbolUses)) ->
                    backgroundTask {
                        let document = document.Project.Solution.GetDocument(documentId)
                        let! sourceText = document.GetTextAsync(cancellationToken)
                        return 
                            [| for symbolUse in symbolUses do
                                    match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                                    | Some span ->
                                        let textSpan = Tokenizer.fixupSpan(sourceText, span)
                                        yield FSharpInlineRenameLocation(document, textSpan) 
                                    | None -> () |]
                    })
                |> Task.WhenAll // Not actually the same, do we need a better way of 'parallelizing' tasks in VS scenarios?
            let locations = Array.concat locations

            return InlineRenameLocationSet(locations, document.Project.Solution, lexerSymbol.Kind, symbolUse.Symbol) :> FSharpInlineRenameLocationSet
        }

[<Export(typeof<FSharpInlineRenameServiceImplementation>); Shared>]
type internal InlineRenameService 
    [<ImportingConstructor>]
    (
    ) =

    inherit FSharpInlineRenameServiceImplementation()

    static member GetInlineRenameInfo(document: Document, position: int) : Async<FSharpInlineRenameInfo option> = 
        asyncMaybe {
            let! ct = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync(ct)
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! symbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof(InlineRenameService))

            let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(nameof(InlineRenameService)) |> Async.AwaitTask |> liftAsync
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland)
            let! declLoc = symbolUse.GetDeclarationLocation(document)

            let! span = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.Range)
            let triggerSpan = Tokenizer.fixupSpan(sourceText, span)

            return InlineRenameInfo(document, triggerSpan, symbol, symbolUse, declLoc, checkFileResults) :> FSharpInlineRenameInfo
        }
    
    override _.GetRenameInfoAsync(document: Document, position: int, _cancellationToken: CancellationToken) : Task<FSharpInlineRenameInfo> =
        backgroundTask {
            let! result = InlineRenameService.GetInlineRenameInfo(document, position)
            return Option.defaultValue null result
        }
