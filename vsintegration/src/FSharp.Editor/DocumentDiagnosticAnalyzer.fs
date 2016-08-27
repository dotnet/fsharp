// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.SolutionCrawler

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

open Microsoft.VisualStudio.FSharp.LanguageService

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpDocumentDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()

    let fSharpErrorToRoslynDiagnostic(document: Document, sourceText: SourceText, error: FSharpErrorInfo) =
        let id = "FS" + error.ErrorNumber.ToString()
        let emptyString = LocalizableString.op_Implicit("")
        let description = LocalizableString.op_Implicit(error.Message)
        let severity = if error.Severity = FSharpErrorSeverity.Error then DiagnosticSeverity.Error else DiagnosticSeverity.Warning
        let descriptor = new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, null)
                
        let linePositionSpan = LinePositionSpan(LinePosition(error.StartLineAlternate - 1, error.StartColumn), LinePosition(error.EndLineAlternate - 1, error.EndColumn))
        let textSpan = sourceText.Lines.GetTextSpan(linePositionSpan)
        let correctedTextSpan = if textSpan.End < sourceText.Length then textSpan else TextSpan.FromBounds(sourceText.Length - 1, sourceText.Length)

        // F# compiler report errors at end of file if parsing fails. It should be corrected to match Roslyn boundaries
        Diagnostic.Create(descriptor, Location.Create(document.FilePath, correctedTextSpan , linePositionSpan))


    // We are constructing our own descriptors at run-time. Compiler service is already doing error formatting and localization.
    override this.SupportedDiagnostics
        with get() =
            let dummyDescriptor = DiagnosticDescriptor("0", String.Empty, String.Empty, String.Empty, DiagnosticSeverity.Error, true, null, null)
            ImmutableArray.Create<DiagnosticDescriptor>(dummyDescriptor)


    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let computation = async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
            let! parseResults = FSharpChecker.Instance.ParseFileInProject(document.Name, sourceText.ToString(), options)

            return (parseResults.Errors |> Seq.map(fun (error) -> fSharpErrorToRoslynDiagnostic(document, sourceText, error))).ToImmutableArray()
        }

        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
             .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)


    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let computation = async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
            let! parseResults = FSharpChecker.Instance.ParseFileInProject(document.Name, sourceText.ToString(), options)
            let! checkResultsAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, document.Name, 0, sourceText.ToString(), options)

            let errors = match checkResultsAnswer with
                         | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                         | FSharpCheckFileAnswer.Succeeded(results) -> results.Errors

            return (errors |> Seq.map(fun (error) -> fSharpErrorToRoslynDiagnostic(document, sourceText, error))).ToImmutableArray()
        }

        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
             .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)
