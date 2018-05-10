// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Diagnostics
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

// IEditorClassificationService is marked as Obsolete, but is still supported. The replacement (IClassificationService)
// is internal to Microsoft.CodeAnalysis.Workspaces which we don't have internals visible to. Rather than add yet another
// IVT, we'll maintain the status quo.
#nowarn "44"

[<ExportLanguageService(typeof<IEditorClassificationService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpClassificationService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    static let userOpName = "SemanticColorization"

    interface IEditorClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member __.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()
        
        member __.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            async {
                Logger.LogBlockMessageStart document.Name LogEditorFunctionId.Classification_Syntactic

                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let! sourceText = document.GetTextAsync(cancellationToken)  |> Async.AwaitTask
                result.AddRange(Tokenizer.getClassifiedSpans(document.Id, sourceText, textSpan, Some(document.FilePath), defines, cancellationToken))

                Logger.LogBlockMessageStop (document.Name + "-Successful") LogEditorFunctionId.Classification_Syntactic

            } |> RoslynHelpers.StartAsyncUnitAsTask cancellationToken

        member __.AddSemanticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            async {
                Logger.LogBlockMessageStart document.Name LogEditorFunctionId.Classification_Semantic

                let! optionsOpt = projectInfoManager.TryGetOptionsForDocumentOrProject(document)

                match optionsOpt with
                | None -> Logger.LogBlockMessageStop (document.Name + "-Failed_OptionsNotFound") LogEditorFunctionId.Classification_Semantic
                | Some(_, _, projectOptions) ->

                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask

                match sourceText with
                | null -> Logger.LogBlockMessageStop (document.Name + "-Failed_SourceTextNotFound") LogEditorFunctionId.Classification_Semantic
                | _ ->

                let! checkResultsOpt = checkerProvider.Checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, allowStaleResults = false, userOpName=userOpName)

                match checkResultsOpt with
                | None -> Logger.LogBlockMessageStop (document.Name + "-Failed_ParseAndCheckDocumentFailed") LogEditorFunctionId.Classification_Semantic
                | Some(_, _, checkResults) ->

                // it's crucial to not return duplicated or overlapping `ClassifiedSpan`s because Find Usages service crashes.
                let targetRange = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)
                let classificationData = checkResults.GetSemanticClassification (Some targetRange) |> Array.distinctBy fst
                
                for (range, classificationType) in classificationData do
                    match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                    | None -> ()
                    | Some span -> 
                        let span = Tokenizer.fixupSpan(sourceText, span)
                        result.Add(ClassifiedSpan(span, FSharpClassificationTypes.getClassificationTypeName(classificationType)))

                Logger.LogBlockMessageStop (document.Name + "-Successful") LogEditorFunctionId.Classification_Semantic
            } 
            |> Async.Ignore |> RoslynHelpers.StartAsyncUnitAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member __.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan



