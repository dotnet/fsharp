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
                
                let textLine = sourceText.Lines.GetLineFromPosition(position)
                let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
                let textLinePos = sourceText.Lines.GetLinePosition(position)
                let fcsTextLineNumber = textLinePos.Line + 1
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, options.OtherOptions |> Seq.toList)
                
                let rangeToDocumentSpan (range: range) =
                    async {
                        match document.Project.Solution.GetDocumentIdsWithFilePath(range.FileName) |> Seq.tryHead with
                        | Some docId ->
                            let doc = document.Project.Solution.GetDocument(docId)
                            let! sourceText = doc.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                            let span = CommonHelpers.fixupSpan(sourceText, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                            return Some (DocumentSpan(doc, span))
                        | None -> return None
                    }
                
                match CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, context.CancellationToken) with
                | Some (islandColumn, qualifiers, _) -> 
                    let! symbolUse = checkFileResults.GetSymbolUseAtLocation(textLineNumber, islandColumn, textLine.ToString(), qualifiers)
                    match symbolUse with
                    | Some symbolUse ->
                        let! declaration = checkFileResults.GetDeclarationLocationAlternate (fcsTextLineNumber, islandColumn, textLine.ToString(), qualifiers, false)
                        let declarationRange = 
                            match declaration with
                            | FSharpFindDeclResult.DeclFound range -> Some range
                            | _ -> None
                        
                        let! declarationSpan =
                            match declarationRange with
                            | Some range -> rangeToDocumentSpan range
                            | None -> async.Return None
                
                    
                        let definitionItem =
                            match declarationSpan with 
                            | Some span ->
                                DefinitionItem.Create(
                                    ImmutableArray<string>.Empty, 
                                    [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray(), 
                                    span)
                            | None -> 
                                DefinitionItem.CreateNonNavigableItem(
                                    ImmutableArray<string>.Empty, 
                                    [TaggedText(TextTags.Text, symbolUse.Symbol.FullName)].ToImmutableArray())    
                
                        do! context.OnDefinitionFoundAsync(definitionItem) |> Async.AwaitTask
                
                        let! projectCheckResults = checker.ParseAndCheckProject(options)
                        let! symbolUses = projectCheckResults.GetUsesOfSymbol(symbolUse.Symbol)
                        for symbolUse in symbolUses do
                            match declarationRange with
                            | Some declRange when declRange = symbolUse.RangeAlternate -> ()
                            | _ ->
                                let! referenceDocSpan = rangeToDocumentSpan symbolUse.RangeAlternate
                                match referenceDocSpan with
                                | Some span -> 
                                    let referenceItem = SourceReferenceItem(definitionItem, span)
                                    do! context.OnReferenceFoundAsync(referenceItem) |> Async.AwaitTask
                                | None -> ()
                    | None -> ()
                | None -> ()
            | None -> ()
            
            do! context.OnCompletedAsync() |> Async.AwaitTask
        }

    interface IStreamingFindReferencesService with
        member __.FindReferencesAsync(document, position, context) =
            findReferencedSymbolsAsync(document, position, context)
            |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 