// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Generic
open System.Collections.Immutable
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.CodeAnalysis.Text

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
        symbolName
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
            | _, ValueSome _ when not allReferences -> ()
            | _, ValueSome textSpan ->
                match textSpan with
                | Tokenizer.FixedSpan sourceText symbolName fixedSpan ->
                    let definitionItem =
                        if isExternal then
                            externalDefinitionItem
                        else
                            definitionItems
                            |> Array.tryFind (fun (_, project: Project) -> project.FilePath = doc.Project.FilePath)
                            |> Option.map (fun (definitionItem, _) -> definitionItem)
                            |> Option.defaultValue externalDefinitionItem

                    let referenceItem =
                        FSharpSourceReferenceItem(definitionItem, FSharpDocumentSpan(doc, fixedSpan))
                    // REVIEW: OnReferenceFoundAsync is throwing inside Roslyn, putting a try/with so find-all refs doesn't fail.
                    try
                        do! onReferenceFoundAsync referenceItem
                    with _ ->
                        ()
                | _ -> ()
        }

    // File can be included in more than one project, hence single `range` may results with multiple `Document`s.
    let rangeToDocumentSpans (solution: Solution, range: range, symbolName: string) =
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

                                match Tokenizer.TryFSharpRangeToTextSpanForEditor(sourceText, range, symbolName) with
                                | ValueSome fixedSpan -> return Some(FSharpDocumentSpan(doc, fixedSpan))
                                | ValueNone -> return None
                            }
                    }
                    |> CancellableTask.whenAll

                return spans |> Array.choose id
            }

    let private referencingCompilationProjects (declaringProject: Project) =
        match declaringProject.OutputFilePath with
        | null -> []
        | outputFilePath ->
            ProjectFiltering.getProjectsReferencingAssembly outputFilePath declaringProject.Solution
            |> List.filter (fun project -> not project.IsFSharp && project.SupportsCompilation)

    /// Locations in a C# or VB project of the symbol with the given documentation comment id.
    let private findRoslynReferences (docId: string) (project: Project) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()

            match! project.GetCompilationAsync cancellationToken with
            | null -> return Seq.empty
            | compilation ->
                match DocumentationCommentId.GetFirstSymbolForDeclarationId(docId, compilation) with
                | null -> return Seq.empty
                | symbol ->
                    let! referencedSymbols =
                        SymbolFinder.FindReferencesAsync(
                            symbol,
                            project.Solution,
                            ImmutableHashSet.CreateRange project.Documents,
                            cancellationToken
                        )

                    return referencedSymbols |> Seq.collect _.Locations
        }

    /// Reports the uses in C# and VB projects that reference the assembly of a project declaring the symbol.
    let private findCrossLanguageReferences
        (docId: string)
        (definitionItems: (FSharpDefinitionItem * Project)[])
        (onReferenceFoundAsync: FSharpSourceReferenceItem -> Task)
        =
        cancellableTask {
            let reported = HashSet<struct (string * TextSpan)>()

            for definitionItem, declaringProject in definitionItems do
                for project in referencingCompilationProjects declaringProject do
                    let! locations = findRoslynReferences docId project

                    for location in locations do
                        let span = location.Location.SourceSpan

                        if reported.Add(struct (location.Document.FilePath, span)) then
                            do!
                                onReferenceFoundAsync (
                                    FSharpSourceReferenceItem(definitionItem, FSharpDocumentSpan(location.Document, span))
                                )
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
                        | Some range -> rangeToDocumentSpans (document.Project.Solution, range, symbol.Ident.idText)
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
                        |> Array.map (fun span -> FSharpDefinitionItem.Create(tags, displayParts, span), span.Document.Project)

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
                            symbol.Ident.idText
                            context.OnReferenceFoundAsync

                    do! SymbolHelpers.findSymbolUses symbolUse document checkFileResults onFound

                    if allReferences && not isExternal && not symbolUse.Symbol.IsInternalToProject then
                        match symbolUse.Symbol.DocumentationCommentId with
                        | ValueSome docId -> do! findCrossLanguageReferences docId definitionItems context.OnReferenceFoundAsync
                        | ValueNone -> ()
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
