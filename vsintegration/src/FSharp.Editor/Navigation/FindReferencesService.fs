// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.CodeAnalysis.FindReferences
open Microsoft.CodeAnalysis.Completion

open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportLanguageService(typeof<IStreamingFindReferencesService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpFindReferencesService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    
    // File can be included in more than one project, hence single `range` may results with multiple `Document`s.
    let rangeToDocumentSpans (solution: Solution, range: range, cancellationToken: CancellationToken) =
        async {
            if range.Start = range.End then return []
            else 
                let! spans =
                    solution.GetDocumentIdsWithFilePath(range.FileName)
                    |> Seq.map (fun documentId ->
                        async {
                            let doc = solution.GetDocument(documentId)
                            let! sourceText = doc.GetTextAsync(cancellationToken)
                            match CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                            | Some span ->
                                let span = CommonHelpers.fixupSpan(sourceText, span)
                                return Some (DocumentSpan(doc, span))
                            | None -> return None
                        })
                    |> Async.Parallel
                return spans |> Array.choose id |> Array.toList
        }

    let findReferencedSymbolsAsync(document: Document, position: int, context: FindReferencesContext) : Async<unit> =
        asyncMaybe {
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let checker = checkerProvider.Checker
            let! options = projectInfoManager.TryGetOptionsForDocumentOrProject(document)
            let! _, checkFileResults = checker.ParseAndCheckDocument(document, options, sourceText)
            let textLine = sourceText.Lines.GetLineFromPosition(position).ToString()
            let lineNumber = sourceText.Lines.GetLinePosition(position).Line + 1
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, options.OtherOptions |> Seq.toList)
            
            let! symbol = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Fuzzy)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(lineNumber, symbol.RightColumn, textLine, [symbol.Text])
            let! declaration = checkFileResults.GetDeclarationLocationAlternate (lineNumber, symbol.RightColumn, textLine, [symbol.Text], false) |> liftAsync
            let tags = GlyphTags.GetTags(CommonRoslynHelpers.GetGlyphForSymbol symbolUse.Symbol)
            
            let declarationRange = 
                match declaration with
                | FSharpFindDeclResult.DeclFound range -> Some range
                | _ -> None
            
            let! declarationSpans =
                match declarationRange with
                | Some range -> rangeToDocumentSpans(document.Project.Solution, range, context.CancellationToken) |> liftAsync
                | None -> async.Return None
            
            let definitionItems =
                match declarationSpans with 
                | [] -> 
                    [ DefinitionItem.CreateNonNavigableItem(
                          tags,
                          [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray(),
                          [TaggedText(TextTags.Assembly, symbolUse.Symbol.Assembly.SimpleName)].ToImmutableArray()) ]
                | _ ->
                    declarationSpans
                    |> List.map (fun span ->
                        DefinitionItem.Create(
                            tags, 
                            [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray(), 
                            span))
            
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
                        | Some (SymbolDeclarationLocation.Projects (declProjects, true)) -> declProjects
                        // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
                        // In order to find all its usages we have to check all F# projects.
                        | _ -> Seq.toList document.Project.Solution.Projects
                
                    asyncMaybe {
                        let! symbolUses =
                            projectsToCheck
                            |> Seq.map (fun project ->
                                asyncMaybe {
                                    let! options = projectInfoManager.TryGetOptionsForProject(project.Id)
                                    let! projectCheckResults = checker.ParseAndCheckProject(options) |> liftAsync
                                    return! projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol) |> liftAsync
                                } |> Async.map (Option.defaultValue [||]))
                            |> Async.Parallel
                            |> liftAsync

                        return symbolUses |> Array.concat
                    }

            for symbolUse in symbolUses do
                match declarationRange with
                | Some declRange when declRange = symbolUse.RangeAlternate -> ()
                | _ ->
                    let! referenceDocSpans = rangeToDocumentSpans(document.Project.Solution, symbolUse.RangeAlternate, context.CancellationToken) |> liftAsync
                    match referenceDocSpans with
                    | [] -> ()
                    | _ ->
                        for referenceDocSpan in referenceDocSpans do
                            for definitionItem in definitionItems do
                                let referenceItem = SourceReferenceItem(definitionItem, referenceDocSpan)
                                do! context.OnReferenceFoundAsync(referenceItem) |> Async.AwaitTask |> liftAsync
            
            do! context.OnCompletedAsync() |> Async.AwaitTask |> liftAsync
        } |> Async.Ignore

    interface IStreamingFindReferencesService with
        member __.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context)
            |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 