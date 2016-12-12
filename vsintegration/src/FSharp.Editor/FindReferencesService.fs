// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.CodeAnalysis.FindReferences

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

    let findReferencedSymbolsAsync(document: Document, position: int, context: FindReferencesContext) : Async<unit> =
        async {
            let! sourceText = document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
            let! textVersion = document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
            let checker = checkerProvider.Checker
            let! options = projectInfoManager.TryGetOptionsForDocumentOrProject(document)
            match options with
            | Some options ->
                let! _parseResults, checkResultsAnswer = checker.ParseAndCheckFileInProject(document.FilePath, hash textVersion, sourceText.ToString(), options)
                let checkFileResults = 
                    match checkResultsAnswer with
                    | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                    | FSharpCheckFileAnswer.Succeeded(results) -> results
                
                let textLine = sourceText.Lines.GetLineFromPosition(position).ToString()
                let lineNumber = sourceText.Lines.GetLinePosition(position).Line + 1
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, options.OtherOptions |> Seq.toList)
                
                // File can be included in more than one project, hence single `range` may results with multiple `Document`s.
                let rangeToDocumentSpans (range: range) =
                    async {
                        if range.Start = range.End then return [||]
                        else 
                            let! spans =
                                document.Project.Solution.GetDocumentIdsWithFilePath(range.FileName)
                                |> Seq.map (fun documentId ->
                                    async {
                                        let doc = document.Project.Solution.GetDocument(documentId)
                                        let! sourceText = doc.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                                        match CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                                        | Some span ->
                                           let span = CommonHelpers.fixupSpan(sourceText, span)
                                           return Some (DocumentSpan(doc, span))
                                        | None -> return None
                                    })
                                |> Async.Parallel
                            return spans |> Array.choose id
                    }
                
                match CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, context.CancellationToken) with
                | Some (islandColumn, qualifiers, _) -> 
                    let! symbolUse = checkFileResults.GetSymbolUseAtLocation(lineNumber, islandColumn, textLine, qualifiers)
                    match symbolUse with
                    | Some symbolUse ->
                        let! declaration = checkFileResults.GetDeclarationLocationAlternate (lineNumber, islandColumn, textLine, qualifiers, false)
                        let declarationRange = 
                            match declaration with
                            | FSharpFindDeclResult.DeclFound range -> Some range
                            | _ -> None
                        
                        let! declarationSpans =
                            match declarationRange with
                            | Some range -> rangeToDocumentSpans range
                            | None -> async.Return [||]
                
                        let definitionItems =
                            match declarationSpans with 
                            | [||] -> 
                                [| DefinitionItem.CreateNonNavigableItem(
                                       ImmutableArray<string>.Empty, 
                                       [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray(),
                                       [TaggedText(TextTags.Assembly, symbolUse.Symbol.Assembly.SimpleName)].ToImmutableArray()) |]
                            | _ ->
                                declarationSpans
                                |> Array.map (fun span ->
                                    DefinitionItem.Create(
                                        ImmutableArray<string>.Empty, 
                                        [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray(), 
                                        span))
                        
                        for definitionItem in definitionItems do
                            do! context.OnDefinitionFoundAsync(definitionItem) |> Async.AwaitTask
                        
                        let! symbolUses =
                            async {
                                if symbolUse.IsPrivateToFile then
                                    return! checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                                else
                                    let! projectCheckResults = checker.ParseAndCheckProject(options)
                                    return! projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                            }

                        for symbolUse in symbolUses do
                            match declarationRange with
                            | Some declRange when declRange = symbolUse.RangeAlternate -> ()
                            | _ ->
                                let! referenceDocSpans = rangeToDocumentSpans symbolUse.RangeAlternate
                                match referenceDocSpans with
                                | [||] -> ()
                                | _ ->
                                    for referenceDocSpan in referenceDocSpans do
                                        for definitionItem in definitionItems do
                                            let referenceItem = SourceReferenceItem(definitionItem, referenceDocSpan)
                                            do! context.OnReferenceFoundAsync(referenceItem) |> Async.AwaitTask
                    | None -> ()
                | None -> ()
            | None -> ()
            
            do! context.OnCompletedAsync() |> Async.AwaitTask
        }

    interface IStreamingFindReferencesService with
        member __.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context)
            |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 