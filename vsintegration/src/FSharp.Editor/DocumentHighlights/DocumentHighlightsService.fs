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

type internal FSharpHighlightSpan =
    { IsDefinition: bool
      TextSpan: TextSpan }
    override this.ToString() = sprintf "%+A" this

[<Shared>]
[<ExportLanguageService(typeof<IDocumentHighlightsService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpDocumentHighlightsService [<ImportingConstructor>] (checkerProvider: FSharpCheckerProvider, projectInfoManager: ProjectInfoManager) =

    /// Fix invalid spans if they appear to have redundant suffix and prefix.
    static let fixInvalidSymbolSpans (sourceText: SourceText) (lastIdent: string) (spans: FSharpHighlightSpan []) =
        spans
        |> Seq.choose (fun (span: FSharpHighlightSpan) ->
            let newLastIdent = sourceText.GetSubText(span.TextSpan).ToString()
            let index = newLastIdent.LastIndexOf(lastIdent, StringComparison.Ordinal)
            if index > 0 then 
                // Sometimes FCS returns a composite identifier for a short symbol, so we truncate the prefix
                // Example: newLastIdent --> "x.Length", lastIdent --> "Length"
                Some { span with TextSpan = TextSpan(span.TextSpan.Start + index, span.TextSpan.Length - index) }
            elif index = 0 && newLastIdent.Length > lastIdent.Length then
                // The returned symbol use is too long; we truncate its redundant suffix
                // Example: newLastIdent --> "Length<'T>", lastIdent --> "Length"
                Some { span with TextSpan = TextSpan(span.TextSpan.Start, lastIdent.Length) }
            elif index = 0 then
                Some span
            else
                // In the case of attributes, a returned symbol use may be a part of original text
                // Example: newLastIdent --> "Sample", lastIdent --> "SampleAttribute"
                let index = lastIdent.LastIndexOf(newLastIdent, StringComparison.Ordinal)
                if index >= 0 then
                    Some span
                else None)
        |> Seq.distinctBy (fun span -> span.TextSpan.Start)
        |> Seq.toArray

    static member GetDocumentHighlights(checker: FSharpChecker, documentKey: DocumentId, sourceText: SourceText, filePath: string, position: int, 
                                        defines: string list, options: FSharpProjectOptions, textVersionHash: int) : Async<FSharpHighlightSpan[]> =
        async {
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = textLinePos.Line + 1
            
            match CommonHelpers.getSymbolAtPosition(documentKey, sourceText, position, filePath, defines, SymbolLookupKind.Fuzzy) with
            | Some symbol -> 
                let! _parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> return [||]
                | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                    let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, textLine.ToString(), [symbol.Text])
                    match symbolUse with
                    | Some symbolUse ->
                        let! symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                        return 
                            [| for symbolUse in symbolUses do
                                 yield { IsDefinition = symbolUse.IsFromDefinition
                                         TextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate) } |]
                            |> fixInvalidSymbolSpans sourceText symbol.Text
                    | None -> return [||]
            | None -> return [||]
        }        

    interface IDocumentHighlightsService with
        member __.GetDocumentHighlightsAsync(document, position, _documentsToSearch, cancellationToken) : Task<ImmutableArray<DocumentHighlights>> =
            async {
                 match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                 | Some options ->
                     let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                     let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                     let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                     let! spans = FSharpDocumentHighlightsService.GetDocumentHighlights(checkerProvider.Checker, document.Id, sourceText, document.FilePath, 
                                                                                        position, defines, options, textVersion.GetHashCode())
                     let highlightSpans = spans |> Array.map (fun span ->
                        let kind = if span.IsDefinition then HighlightSpanKind.Definition else HighlightSpanKind.Reference
                        HighlightSpan(span.TextSpan, kind))
                     
                     return [| DocumentHighlights(document, highlightSpans.ToImmutableArray()) |].ToImmutableArray()
                 | None -> return ImmutableArray<DocumentHighlights>()
            }   
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
