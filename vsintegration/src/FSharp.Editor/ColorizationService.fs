// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Classification
open Microsoft.CodeAnalysis.Editor.Implementation.BlockCommentEditing
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Operations
open Microsoft.VisualStudio.Utilities

open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportLanguageService(typeof<IEditorClassificationService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpColorizationService() =
    interface IEditorClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member this.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()
        
        member this.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            async {
                let defines = FSharpLanguageService.GetCompilationDefinesForEditingDocument(document)  
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                result.AddRange(CommonHelpers.getColorizationData(document.Id, sourceText, textSpan, Some(document.FilePath), defines, cancellationToken))
            } |> CommonRoslynHelpers.StartAsyncUnitAsTask cancellationToken

        member this.AddSemanticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            async {
                match FSharpLanguageService.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                    let! _parseResults, checkResultsAnswer = FSharpLanguageService.Checker.ParseAndCheckFileInProject(document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)

                    let extraColorizationData = 
                        match checkResultsAnswer with
                        | FSharpCheckFileAnswer.Aborted -> [| |]
                        | FSharpCheckFileAnswer.Succeeded(results) -> 
                            [| for (range, tokenColorKind)  in results.GetExtraColorizationsAlternate() do
                                  let span = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range)
                                  if textSpan.Contains(span.Start) || textSpan.Contains(span.End - 1) || span.Contains(textSpan) then
                                      yield ClassifiedSpan(span, CommonHelpers.compilerTokenToRoslynToken(tokenColorKind)) |]

                    result.AddRange(extraColorizationData)
                | None -> ()
            } |> CommonRoslynHelpers.StartAsyncUnitAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member this.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan



