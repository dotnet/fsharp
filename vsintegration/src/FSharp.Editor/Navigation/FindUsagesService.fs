// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open CancellableTasks

module FSharpFindUsagesService =

    let onSymbolFound
        allReferences
        declarationRange
        externalDefinitionItem
        definitionItems
        isExternal
        (onReferenceFoundAsync: FSharpSourceReferenceItem -> Task)
        (doc: Document)
        (symbolUse: range)
        =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! sourceText = doc.GetTextAsync(cancellationToken)

            match declarationRange, RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
            | Some declRange, _ when Range.equals declRange symbolUse -> ()
            | _, ValueNone -> ()
            | _, ValueSome textSpan ->
                if allReferences then
                    let definitionItem =
                        if isExternal then
                            externalDefinitionItem
                        else
                            definitionItems
                            |> Array.tryFind (snd >> (=) doc.Project.FilePath)
                            |> Option.map (fun (definitionItem, _) -> definitionItem)
                            |> Option.defaultValue externalDefinitionItem

                    let referenceItem =
                        FSharpSourceReferenceItem(definitionItem, FSharpDocumentSpan(doc, textSpan))
                    // REVIEW: OnReferenceFoundAsync is throwing inside Roslyn, putting a try/with so find-all refs doesn't fail.
                    try
                        do! onReferenceFoundAsync referenceItem
                    with _ ->
                        ()
        }

    // File can be included in more than one project, hence single `range` may results with multiple `Document`s.
    let rangeToDocumentSpans (solution: Solution, range: range) =
        if range.Start = range.End then
            CancellableTask.singleton [||]
        else
            cancellableTask {
                let documentIds = solution.GetDocumentIdsWithFilePath(range.FileName)

                let! spans =
                    seq {
                        for documentId in documentIds do
                            cancellableTask {
                                let doc = solution.GetDocument(documentId)
                                let! cancellationToken = CancellableTask.getCancellationToken ()
                                let! sourceText = doc.GetTextAsync(cancellationToken)

                                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                                | ValueSome span ->
                                    let span = Tokenizer.fixupSpan (sourceText, span)
                                    return Some(FSharpDocumentSpan(doc, span))
                                | ValueNone -> return None
                            }
                    }
                    |> CancellableTask.whenAll

                return spans |> Array.choose id
            }

    let findReferencedSymbolsAsync
        (document: Document, position: int, context: IFSharpFindUsagesContext, allReferences: bool, userOp: string)
        : CancellableTask<unit> =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync(cancellationToken)
            let textLine = sourceText.Lines.GetLineFromPosition(position).ToString()
            let lineNumber = sourceText.Lines.GetLinePosition(position).Line + 1

            match! document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOp) with
            | None -> ()
            | Some symbol ->

                let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOp)

                let symbolUse =
                    checkFileResults.GetSymbolUseAtLocation(lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland)

                let declaration =
                    checkFileResults.GetDeclarationLocation(lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland, false)

                match symbolUse with
                | None -> ()
                | Some symbolUse ->

                    let tags =
                        FSharpGlyphTags.GetTags(Tokenizer.GetGlyphForSymbol(symbolUse.Symbol, symbol.Kind))

                    let declarationRange =
                        match declaration with
                        | FindDeclResult.DeclFound range -> Some range
                        | _ -> None

                    let! declarationSpans =
                        match declarationRange with
                        | Some range -> rangeToDocumentSpans (document.Project.Solution, range)
                        | None -> CancellableTask.singleton [||]

                    let declarationSpans =
                        declarationSpans
                        |> Array.distinctBy (fun x -> x.Document.FilePath, x.Document.Project.FilePath)

                    let isExternal = Array.isEmpty declarationSpans

                    let displayParts =
                        ImmutableArray.Create(Microsoft.CodeAnalysis.TaggedText(TextTags.Text, symbol.Ident.idText))

                    let originationParts =
                        ImmutableArray.Create(Microsoft.CodeAnalysis.TaggedText(TextTags.Assembly, symbolUse.Symbol.Assembly.SimpleName))

                    let externalDefinitionItem =
                        FSharpDefinitionItem.CreateNonNavigableItem(tags, displayParts, originationParts)

                    let definitionItems =
                        declarationSpans
                        |> Array.map (fun span -> FSharpDefinitionItem.Create(tags, displayParts, span), span.Document.Project.FilePath)

                    do!
                        definitionItems
                        |> Seq.map (fst >> context.OnDefinitionFoundAsync)
                        |> Task.WhenAll

                    if isExternal then
                        do! context.OnDefinitionFoundAsync(externalDefinitionItem)

                    let onFound =
                        onSymbolFound
                            allReferences
                            declarationRange
                            externalDefinitionItem
                            definitionItems
                            isExternal
                            context.OnReferenceFoundAsync

                    do! SymbolHelpers.findSymbolUses symbolUse document checkFileResults onFound
        }

open FSharpFindUsagesService

[<Export(typeof<IFSharpFindUsagesService>)>]
type internal FSharpFindUsagesService [<ImportingConstructor>] () =
    interface IFSharpFindUsagesService with
        member _.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync (document, position, context, true, nameof (FSharpFindUsagesService))
            |> CancellableTask.startAsTask context.CancellationToken

        member _.FindImplementationsAsync(document, position, context) =
            findReferencedSymbolsAsync (document, position, context, false, nameof (FSharpFindUsagesService))
            |> CancellableTask.startAsTask context.CancellationToken
