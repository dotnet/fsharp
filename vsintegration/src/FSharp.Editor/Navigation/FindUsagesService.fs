// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages

open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices

[<Export(typeof<IFSharpFindUsagesService>)>]
type internal FSharpFindUsagesService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    static let userOpName = "FindUsages"

    // File can be included in more than one project, hence single `range` may results with multiple `Document`s.
    let rangeToDocumentSpans (solution: Solution, range: range) =
        async {
            if range.Start = range.End then return []
            else 
                let! spans =
                    solution.GetDocumentIdsWithFilePath(range.FileName)
                    |> Seq.map (fun documentId ->
                        async {
                            let doc = solution.GetDocument(documentId)
                            let! cancellationToken = Async.CancellationToken
                            let! sourceText = doc.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                            | Some span ->
                                let span = Tokenizer.fixupSpan(sourceText, span)
                                return Some (FSharpDocumentSpan(doc, span))
                            | None -> return None
                        })
                    |> Async.Parallel
                return spans |> Array.choose id |> Array.toList
        }

    let findReferencedSymbolsAsync(document: Document, position: int, context: IFSharpFindUsagesContext, allReferences: bool, userOpName: string) : Async<unit> =
        asyncMaybe {
            let! sourceText = document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask |> liftAsync
            let checker = checkerProvider.Checker
            let! parsingOptions, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject(document, context.CancellationToken)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName = userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition(position).ToString()
            let lineNumber = sourceText.Lines.GetLinePosition(position).Line + 1
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            
            let! symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland, userOpName=userOpName)
            let! declaration = checkFileResults.GetDeclarationLocation (lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland, false, userOpName=userOpName) |> liftAsync
            let tags = GlyphTags.GetTags(Microsoft.CodeAnalysis.ExternalAccess.FSharp.FSharpGlyphHelpersObsolete.Convert(Tokenizer.GetGlyphForSymbol (symbolUse.Symbol, symbol.Kind)))
            
            let declarationRange = 
                match declaration with
                | FSharpFindDeclResult.DeclFound range -> Some range
                | _ -> None
            
            let! definitionItems =
                async {
                    let! declarationSpans =
                        match declarationRange with
                        | Some range -> rangeToDocumentSpans(document.Project.Solution, range)
                        | None -> async.Return []
                    
                    return 
                        match declarationSpans with 
                        | [] -> 
                            [ FSharpDefinitionItem.CreateNonNavigableItem(
                                tags,
                                ImmutableArray.Create(TaggedText(TextTags.Text, symbol.Ident.idText)),
                                ImmutableArray.Create(TaggedText(TextTags.Assembly, symbolUse.Symbol.Assembly.SimpleName))) ]
                        | _ ->
                            declarationSpans
                            |> List.map (fun span ->
                                FSharpDefinitionItem.Create(tags, ImmutableArray.Create(TaggedText(TextTags.Text, symbol.Ident.idText)), span))
                } |> liftAsync
            
            for definitionItem in definitionItems do
                do! context.OnDefinitionFoundAsync(definitionItem) |> Async.AwaitTask |> liftAsync
            
            let! symbolUses =
                match symbolUse.GetDeclarationLocation document with
                | Some SymbolDeclarationLocation.CurrentDocument ->
                    checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol) |> liftAsync
                | scope ->
                    let projectsToCheck =
                        match scope with
                        | Some (SymbolDeclarationLocation.Projects (declProjects, false)) ->
                            [ for declProject in declProjects do
                                yield declProject
                                yield! declProject.GetDependentProjects() ]
                            |> List.distinct
                        | Some (SymbolDeclarationLocation.Projects (declProjects, true)) -> declProjects
                        // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
                        // In order to find all its usages we have to check all F# projects.
                        | _ -> Seq.toList document.Project.Solution.Projects
                
                    SymbolHelpers.getSymbolUsesInProjects (symbolUse.Symbol, projectInfoManager, checker, projectsToCheck, userOpName) |> liftAsync

            for symbolUse in symbolUses do
                match declarationRange with
                | Some declRange when declRange = symbolUse.RangeAlternate -> ()
                | _ ->
                    // report a reference if we're interested in all _or_ if we're looking at an implementation
                    if allReferences || symbolUse.IsFromDispatchSlotImplementation then
                        let! referenceDocSpans = rangeToDocumentSpans(document.Project.Solution, symbolUse.RangeAlternate) |> liftAsync
                        match referenceDocSpans with
                        | [] -> ()
                        | _ ->
                            for referenceDocSpan in referenceDocSpans do
                                for definitionItem in definitionItems do
                                    let referenceItem = FSharpSourceReferenceItem(definitionItem, referenceDocSpan)
                                    do! context.OnReferenceFoundAsync(referenceItem) |> Async.AwaitTask |> liftAsync
            
            ()
        } |> Async.Ignore

    interface IFSharpFindUsagesService with
        member __.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context, true, userOpName)
            |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
        member __.FindImplementationsAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context, false, userOpName)
            |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 