// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.FindSymbols

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open CancellableTasks

module FSharpFindUsagesService =

    // REVIEW: OnReferenceFoundAsync is throwing inside Roslyn, putting a try/with so find-all refs doesn't fail.
    let private reportReference (onReferenceFoundAsync: FSharpSourceReferenceItem -> Task) definitionItem (document: Document) span =
        cancellableTask {
            try
                do! onReferenceFoundAsync (FSharpSourceReferenceItem(definitionItem, FSharpDocumentSpan(document, span)))
            with _ ->
                ()
        }

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
                            |> Array.tryFindV (fun struct (_, project: Project) -> project.FilePath = doc.Project.FilePath)
                            |> ValueOption.map (fun struct (definitionItem, _) -> definitionItem)
                            |> ValueOption.defaultValue externalDefinitionItem

                    do! reportReference onReferenceFoundAsync definitionItem doc fixedSpan
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

    // DocumentationCommentId alone searches only the unaliased references, and finds a consumer's namesake first.
    let private symbolOfId (docId: string) (assembly: IAssemblySymbol) =
        let names = docId.Substring(2).Split('(').[0].Split('.')

        (Seq.singleton (assembly.GlobalNamespace :> INamespaceOrTypeSymbol), Array.take (names.Length - 1) names)
        ||> Seq.fold (fun containers name ->
            containers
            |> Seq.collect _.GetMembers()
            |> Seq.chooseV (function
                | :? INamespaceOrTypeSymbol as container when container.MetadataName = name -> ValueSome container
                | _ -> ValueNone))
        |> Seq.collect _.GetMembers()
        |> Seq.tryFindV (fun candidate -> DocumentationCommentId.CreateDeclarationId(candidate).Split('~').[0] = docId)

    let private findRoslynReferences (docId: string) (declaringAssembly: string) (project: Project) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()

            match! project.GetCompilationAsync cancellationToken with
            | null -> return Seq.empty
            | compilation ->
                let ofDeclaringAssembly =
                    compilation.SourceModule.ReferencedAssemblySymbols
                    |> Seq.tryFindV (fun assembly -> String.Equals(assembly.Name, declaringAssembly, StringComparison.OrdinalIgnoreCase))
                    |> ValueOption.bind (symbolOfId docId)

                match ofDeclaringAssembly with
                | ValueNone -> return Seq.empty
                | ValueSome symbol ->
                    let! referencedSymbols =
                        SymbolFinder.FindReferencesAsync(
                            symbol,
                            project.Solution,
                            ImmutableHashSet.CreateRange project.Documents,
                            cancellationToken
                        )

                    return referencedSymbols |> Seq.collect _.Locations
        }

    [<Literal>]
    let private ConcurrentCompilations = 4

    let private findCrossLanguageReferences (docId: string) (definitionItems: struct (FSharpDefinitionItem * Project)[]) =
        seq {
            for struct (definitionItem, declaringProject) in definitionItems do
                for project in ProjectFiltering.getProjectsReferencingAssembly declaringProject.OutputFilePath declaringProject.Solution do
                    if project.SupportsCompilation && not project.IsFSharp then
                        struct (definitionItem, declaringProject.AssemblyName, project)
        }
        |> Seq.distinctBy (fun struct (_, _, project) -> project.Id)
        |> Seq.map (fun struct (definitionItem, declaringAssembly, project) ->
            findRoslynReferences docId declaringAssembly project
            |> CancellableTask.map (Seq.map (fun location -> struct (definitionItem, location))))
        |> CancellableTask.whenAllThrottled ConcurrentCompilations
        // Each file span once: the target-framework instances of a consumer share their files.
        |> CancellableTask.map (fun found ->
            found
            |> Seq.concat
            |> Seq.distinctBy (fun struct (_, location) -> struct (location.Document.FilePath, location.Location.SourceSpan)))

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
                        |> Array.map (fun span -> struct (FSharpDefinitionItem.Create(tags, displayParts, span), span.Document.Project))

                    do!
                        definitionItems
                        |> Seq.map (fun struct (definitionItem, _) -> context.OnDefinitionFoundAsync definitionItem)
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

                    let crossLanguageSearch =
                        match symbolUse.Symbol.DocumentationCommentId with
                        | ValueSome docId when allReferences && not isExternal && not symbolUse.Symbol.IsInternalToProject ->
                            findCrossLanguageReferences docId definitionItems
                        | _ -> CancellableTask.singleton Seq.empty

                    let! _, found =
                        CancellableTask.parallelZip
                            (SymbolHelpers.findSymbolUses symbolUse document checkFileResults onFound)
                            crossLanguageSearch

                    for struct (definitionItem, location) in found do
                        do! reportReference context.OnReferenceFoundAsync definitionItem location.Document location.Location.SourceSpan
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
