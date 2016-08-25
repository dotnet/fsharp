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
    
    // We are constructing our own descriptors at run-time. Compiler service is already doing error formatting and localization.
    override this.SupportedDiagnostics with get() = ImmutableArray<DiagnosticDescriptor>.Empty

    override this.AnalyzeSyntaxAsync(_, _): Task<ImmutableArray<Diagnostic>> =
        Task.FromResult(ImmutableArray<Diagnostic>.Empty)

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let computation = async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
            let! parseResults = FSharpChecker.Instance.ParseFileInProject(document.Name, sourceText.ToString(), options)
            let! checkResultsAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, document.Name, 0, sourceText.ToString(), options)

            let errors = match checkResultsAnswer with
                         | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                         | FSharpCheckFileAnswer.Succeeded(results) -> results.Errors

            let diagnostics = errors |> Seq.map(fun (error) ->
                let id = "FS" + error.ErrorNumber.ToString()
                let emptyString = LocalizableString.op_Implicit("")
                let description = LocalizableString.op_Implicit(error.Message)
                let severity = if error.Severity = FSharpErrorSeverity.Error then DiagnosticSeverity.Error else DiagnosticSeverity.Warning
                let descriptor = new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, null)

                let location = match (error.StartLineAlternate - 1, error.EndLineAlternate - 1) with 
                               | (-1, _) -> Location.None
                               | (_, -1) -> Location.None
                               | (startl, endl) ->
                                    let linePositionSpan = LinePositionSpan(LinePosition(startl, error.StartColumn), LinePosition(endl, error.EndColumn))
                                    Location.Create(error.FileName, sourceText.Lines.GetTextSpan(linePositionSpan) , linePositionSpan)

                Diagnostic.Create(descriptor, location))
            return Seq.toArray(diagnostics).ToImmutableArray()
        }

        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
             .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)
