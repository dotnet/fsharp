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
open CancellableTasks

type internal InlineRenameReplacementInfo(newSolution: Solution, replacementTextValid: bool, documentIds: IEnumerable<DocumentId>) =
    inherit FSharpInlineRenameReplacementInfo()

    override _.NewSolution = newSolution
    override _.ReplacementTextValid = replacementTextValid
    override _.DocumentIds = documentIds
    override _.GetReplacements _ = Seq.empty

type internal InlineRenameLocationSet
    (
        locations: FSharpInlineRenameLocation[],
        originalSolution: Solution,
        symbolKind: LexerSymbolKind,
        symbol: FSharpSymbol
    ) =

    inherit FSharpInlineRenameLocationSet()

    static let rec applyChanges replacementText (solution: Solution) (locationsByDocument: (Document * FSharpInlineRenameLocation list) list) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
            match locationsByDocument with
            | [] -> return solution
            | (document, locations) :: rest ->
                let! oldSource = document.GetTextAsync(cancellationToken)

                let newSource =
                    oldSource.WithChanges(locations |> List.map (fun l -> TextChange(l.TextSpan, replacementText)))

                return! applyChanges replacementText (solution.WithDocumentText(document.Id, newSource)) rest
        }

    override _.Locations = upcast locations.ToList()

    override _.GetReplacementsAsync(replacementText, cancellationToken) : Task<FSharpInlineRenameReplacementInfo> =

        cancellableTask {
            let! newSolution = applyChanges replacementText originalSolution (locations |> Array.toList |> List.groupBy (fun x -> x.Document))

            let replacementText =
                match symbolKind with
                | LexerSymbolKind.GenericTypeParameter
                | LexerSymbolKind.StaticallyResolvedTypeParameter -> replacementText
                | _ -> FSharpKeywords.NormalizeIdentifierBackticks replacementText

            let replacementTextValid =
                Tokenizer.isValidNameForSymbol (symbolKind, symbol, replacementText)

            let documentIds = locations |> Seq.map (fun doc -> doc.Document.Id) |> Seq.distinct
            return new InlineRenameReplacementInfo(newSolution, replacementTextValid, documentIds) :> FSharpInlineRenameReplacementInfo
        }
        |> CancellableTask.start cancellationToken

type internal InlineRenameInfo
    (
        document: Document,
        triggerSpan: TextSpan,
        sourceText: SourceText,
        lexerSymbol: LexerSymbol,
        symbolUse: FSharpSymbolUse,
        checkFileResults: FSharpCheckFileResults,
        ct: CancellationToken
    ) =

    inherit FSharpInlineRenameInfo()

    let getDocumentText (document: Document) =
        match document.TryGetText() with
        | true, text -> CancellableTask.singleton text
        | _ ->
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                return! document.GetTextAsync(cancellationToken)
            }

    let symbolUses =
        SymbolHelpers.getSymbolUsesInSolution (symbolUse, checkFileResults, document) ct

    override _.CanRename = true
    override _.LocalizedErrorMessage = null
    override _.TriggerSpan = triggerSpan
    override _.HasOverloads = false
    override _.ForceRenameOverloads = false
    override _.DisplayName = symbolUse.Symbol.DisplayName

    override _.FullDisplayName =
        try
            symbolUse.Symbol.FullName
        with _ ->
            symbolUse.Symbol.DisplayName

    override _.Glyph = Glyph.MethodPublic
    override _.GetFinalSymbolName replacementText = replacementText

    override _.DefinitionLocations =
        ImmutableArray.Create(new FSharpInlineRenameLocation(document, triggerSpan))

    override _.GetReferenceEditSpan(location, cancellationToken) =

        let text =
            if location.Document = document then
                sourceText
            else
                let textTask = getDocumentText location.Document
                CancellableTask.runSyncronously cancellationToken textTask

        Tokenizer.fixupSpan (text, location.TextSpan)

    override _.GetConflictEditSpan(location, replacementText, cancellationToken) =
        let text =
            if location.Document = document then
                sourceText
            else
                let textTask = getDocumentText location.Document
                CancellableTask.runSyncronously cancellationToken textTask

        let spanText = text.ToString(location.TextSpan)
        let position = spanText.LastIndexOf(replacementText, StringComparison.Ordinal)

        if position < 0 then
            Nullable()
        else
            Nullable(TextSpan(location.TextSpan.Start + position, replacementText.Length))

    override _.FindRenameLocationsAsync(_, _, cancellationToken) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
            let! symbolUsesByDocumentId = symbolUses

            let locationTasks =
                [|
                    for (KeyValue (documentId, symbolUses)) in symbolUsesByDocumentId do
                        let t =
                            cancellableTask {
                                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                                let document = document.Project.Solution.GetDocument(documentId)
                                let! sourceText = document.GetTextAsync(cancellationToken)

                                return
                                    [|
                                        for symbolUse in symbolUses do
                                            match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                                            | Some span ->
                                                let textSpan = Tokenizer.fixupSpan (sourceText, span)
                                                yield FSharpInlineRenameLocation(document, textSpan)
                                            | None -> ()
                                    |]
                            }

                        yield CancellableTask.start cancellationToken t
                |]

            let! results = Task.WhenAll locationTasks

            let locations = Array.concat results

            return
                InlineRenameLocationSet(locations, document.Project.Solution, lexerSymbol.Kind, symbolUse.Symbol)
                :> FSharpInlineRenameLocationSet
        }
        |> CancellableTask.start cancellationToken

[<Export(typeof<FSharpInlineRenameServiceImplementation>); Shared>]
type internal InlineRenameService [<ImportingConstructor>] () =

    inherit FSharpInlineRenameServiceImplementation()

    override _.GetRenameInfoAsync(document: Document, position: int, cancellationToken: CancellationToken) : Task<FSharpInlineRenameInfo> =
        cancellableTask {
            let! ct = CancellableTask.getCurrentCancellationToken ()
            let! sourceText = document.GetTextAsync(ct)
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! symbol =
                document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof (InlineRenameService))

            // TODO: Rewrite to less nested variant after everything works.
            match symbol with
            | None -> return Unchecked.defaultof<_>
            | Some symbol ->
                let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(nameof (InlineRenameService))

                let symbolUse =
                    checkFileResults.GetSymbolUseAtLocation(
                        fcsTextLineNumber,
                        symbol.Ident.idRange.EndColumn,
                        textLine.Text.ToString(),
                        symbol.FullIsland
                    )

                match symbolUse with
                | None -> return Unchecked.defaultof<_>
                | Some symbolUse ->
                    let span = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.Range)

                    match span with
                    | None -> return Unchecked.defaultof<_>
                    | Some span ->
                        let triggerSpan = Tokenizer.fixupSpan (sourceText, span)

                        return
                            InlineRenameInfo(document, triggerSpan, sourceText, symbol, symbolUse, checkFileResults, ct)
                            :> FSharpInlineRenameInfo
        }
        |> CancellableTask.start cancellationToken
