// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.ReferenceHighlighting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Windows.Documents
 
[<System.Composition.Shared>]
[<ExportLanguageService(typeof<IDocumentHighlightsService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpDocumentHighlightsService 
    [<System.Composition.ImportingConstructor>] 
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    static member GetDocumentHighlights(checker: FSharpChecker, documentKey: DocumentId, sourceText: SourceText, filePath: string, position: int, 
                                        defines: string list, options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) 
                                        : Async<HighlightSpan[]> =
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1
            let textLineColumn = textLinePos.Character
            let tryGetHighlightsAtPosition position =
                async {
                    match CommonHelpers.tryClassifyAtPosition(documentKey, sourceText, filePath, defines, position, cancellationToken) with 
                    | Some (islandColumn, qualifiers, _) -> 
                        let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
                        match checkFileAnswer with
                        | FSharpCheckFileAnswer.Aborted -> return [||]
                        | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, islandColumn, textLine.ToString(), qualifiers)
                            match symbolUse with
                            | Some symbolUse ->
                                let! symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                                return 
                                    [| for symbolUse in symbolUses do
                                         let kind = if symbolUse.IsFromDefinition then HighlightSpanKind.Definition else HighlightSpanKind.Reference
                                         yield HighlightSpan(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate), kind) |]
                            | None -> return [||]
                    | None -> return [||]
                }
            let! attempt1 = tryGetHighlightsAtPosition position
            match attempt1 with
            | [||] when textLineColumn > 0 -> return! tryGetHighlightsAtPosition (position - 1)
            | res -> return res
        }        

    interface IDocumentHighlightsService with
        member __.GetDocumentHighlightsAsync(document, position, _documentsToSearch, cancellationToken) : Task<ImmutableArray<DocumentHighlights>> =
            async {
                 match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                 | Some options ->
                     let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                     let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                     let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                     let! highlightSpans = FSharpDocumentHighlightsService.GetDocumentHighlights(checkerProvider.Checker, document.Id, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode(), cancellationToken)
                     return [| DocumentHighlights(document, highlightSpans.ToImmutableArray()) |].ToImmutableArray()
                 | None -> return ImmutableArray<DocumentHighlights>()
            }   
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
