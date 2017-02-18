// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Threading
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportLanguageService(typeof<IEditorClassificationService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpColorizationService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    interface IEditorClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member this.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()
        
        member this.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            Logging.Logging.logInfof "=> FSharpColorizationService.GetItemsAsync\n%s" Environment.StackTrace
            async {
                use! __ = Async.OnCancel(fun () -> Logging.Logging.logInfof "CANCELLED FSharpColorizationService.AddSyntacticClassificationsAsync\n%s" Environment.StackTrace)
                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let! sourceText = document.GetTextAsync(cancellationToken)
                result.AddRange(CommonHelpers.getColorizationData(document.Id, sourceText, textSpan, Some(document.FilePath), defines, cancellationToken))
            } |> CommonRoslynHelpers.StartAsyncUnitAsTask cancellationToken

        member this.AddSemanticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            Logging.Logging.logInfof "=> FSharpColorizationService.AddSemanticClassificationsAsync\n%s" Environment.StackTrace
            asyncMaybe {
                use! __ = Async.OnCancel(fun () -> Logging.Logging.logInfof "CANCELLED FSharpColorizationService.AddSemanticClassificationsAsync\n%s" Environment.StackTrace) |> liftAsync
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! _, _, checkResults = checkerProvider.Checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = false) 
                // it's crucial to not return duplicated or overlapping `ClassifiedSpan`s because Find Usages service crashes.
                let targetRange = CommonRoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)
                let colorizationData = checkResults.GetSemanticClassification (Some targetRange) |> Array.distinctBy fst
                
                for (range, classificationType) in colorizationData do
                    let span = CommonHelpers.fixupSpan(sourceText, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                    result.Add(ClassifiedSpan(span, FSharpClassificationTypes.getClassificationTypeName(classificationType)))
            } 
            |> Async.Ignore |> CommonRoslynHelpers.StartAsyncUnitAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member this.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan



