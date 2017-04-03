// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Editor.FindUsages
open Microsoft.CodeAnalysis.FindUsages

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportLanguageService(typeof<IFindUsagesService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpFindUsagesService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    
    // File can be included in more than one project.
    let rangeToDocumentSpan (solution: Solution, range: range, cancellationToken: CancellationToken) =
        async {
            if range.Start = range.End then return []
            else 
                let! spans =
                    solution.GetDocumentIdsWithFilePath(range.FileName)
                    |> Seq.map (fun documentId ->
                        asyncMaybe {
                            let doc = solution.GetDocument(documentId)
                            let! sourceText = doc.GetTextAsync(cancellationToken)
                            match CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                            | Some span ->
                                let span = CommonHelpers.fixupSpan(sourceText, span)
                                return DocumentSpan(doc, span)
                            | None -> return! None
                            
                        })
                    |> Async.Parallel
                return spans |> Array.choose id |> Array.toList
        }

    let findReferencedSymbolsAsync(document: Document, position: int, context: IFindUsagesContext, allReferences: bool) : Async<unit> =
        asyncMaybe {
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let checker = checkerProvider.Checker
            let! options = projectInfoManager.TryGetOptionsForDocumentOrProject(document)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
            let textLine = sourceText.Lines.GetLineFromPosition(position).ToString()
            let lineNumber = sourceText.Lines.GetLinePosition(position).Line + 1
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, options.OtherOptions |> Seq.toList)
            
            let! symbol = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland)
            let! declaration = checkFileResults.GetDeclarationLocationAlternate (lineNumber, symbol.Ident.idRange.EndColumn, textLine, symbol.FullIsland, false) |> liftAsync
            let tags = GlyphTags.GetTags(CommonRoslynHelpers.GetGlyphForSymbol (symbolUse.Symbol, symbol.Kind))
            
            let declaration = 
                match declaration with
                | FSharpFindDeclResult.DeclFound (range, assemblyName) -> Some (range, assemblyName)
                | _ -> None
            
            let! declarationSpan =
                async {
                    match declaration with
                    | Some (range, assembly) -> 
                        let! spans = rangeToDocumentSpan(document.Project.Solution, range, context.CancellationToken)
                        // A file may be part of multiple projects. We are interested in the one own declaration symbol is from.
                        return spans |> List.tryFind (fun span -> span.Document.Project.AssemblyName = assembly)
                    | None -> return None
                } |> liftAsync
                    
            let definitionItem =
                match declarationSpan with 
                | None -> 
                    DefinitionItem.CreateNonNavigableItem(
                        tags,
                        ImmutableArray.Create(TaggedText(TextTags.Text, symbol.Ident.idText)),
                        ImmutableArray.Create(TaggedText(TextTags.Assembly, symbolUse.Symbol.Assembly.SimpleName)))
                | Some span ->
                    DefinitionItem.Create(tags, ImmutableArray.Create(TaggedText(TextTags.Text, symbol.Ident.idText)), span)
            
            do! context.OnDefinitionFoundAsync(definitionItem) |> Async.AwaitTask |> liftAsync
            
            let! symbolUses =
                match declarationSpan with
                | Some _ when symbolUse.IsPrivateToFile ->
                    asyncMaybe {
                        let! symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol) |> liftAsync
                        return symbolUses |> Array.map (fun x -> document.Project, x)
                    }
                | _ ->
                    let projectsToCheck =
                        match declarationSpan with
                        | Some declSpan when symbolUse.Symbol.IsInternalToProject ->
                            [declSpan.Document.Project]
                        | Some declSpan ->
                            [ yield declSpan.Document.Project
                              yield! declSpan.Document.Project.GetDependentProjects() ]
                        | None ->
                            // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
                            // In order to find all its usages we have to check all F# projects.
                            Seq.toList document.Project.Solution.Projects
                
                    asyncMaybe {
                        let! symbolUses =
                            projectsToCheck
                            |> Seq.map (fun project ->
                                asyncMaybe {
                                    let! options = projectInfoManager.TryGetOptionsForProject(project.Id)
                                    let! projectCheckResults = checker.ParseAndCheckProject(options) |> liftAsync
                                    let! symbolUses = projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol) |> liftAsync
                                    // FCS may return several `FSharpSymbolUse`s for same range, which have different `ItemOccurrence`s (Use, UseInAttribute, UseInType, etc.)
                                    // We don't care about the occurrence type here, so we distinct by range.
                                    return symbolUses |> Array.map (fun x -> project, x)
                                } |> Async.map (Option.defaultValue [||]))
                            |> Async.Parallel
                            |> liftAsync

                        return symbolUses |> Array.concat |> Array.distinctBy (fun (_, x) -> x.RangeAlternate)
                    }

            for symbolUseProject, symbolUse in symbolUses do
                match declaration with
                | Some (declRange, _) when declRange = symbolUse.RangeAlternate -> ()
                | _ ->
                    // report a reference if we're interested in all _or_ if we're looking at an implementation
                    if allReferences || symbolUse.IsFromDispatchSlotImplementation then
                        let! referenceDocSpans = 
                            rangeToDocumentSpan(document.Project.Solution, symbolUse.RangeAlternate, context.CancellationToken) |> liftAsync
                        
                        match referenceDocSpans |> List.filter (fun x -> x.Document.Project = symbolUseProject) with
                        | [] -> ()
                        | _ ->
                            for referenceDocSpan in referenceDocSpans do
                                let referenceItem = SourceReferenceItem(definitionItem, referenceDocSpan)
                                do! context.OnReferenceFoundAsync(referenceItem) |> Async.AwaitTask |> liftAsync
            
            ()
        } |> Async.Ignore

    interface IFindUsagesService with
        member __.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context, true)
            |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
        member __.FindImplementationsAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context, false)
            |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 