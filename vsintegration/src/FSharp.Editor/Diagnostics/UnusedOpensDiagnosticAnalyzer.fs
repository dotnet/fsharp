// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Diagnostics
open System.Threading

open Microsoft.CodeAnalysis

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

[<Export(typeof<IFSharpUnusedOpensDiagnosticAnalyzer>)>]
type internal UnusedOpensDiagnosticAnalyzer
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    static let userOpName = "UnusedOpensAnalyzer"

    static member GetUnusedOpenRanges(document: Document, options, checker: FSharpChecker) : Async<Option<_ * _ * range list>> =
        asyncMaybe {
            do! Option.guard document.FSharpOptions.CodeFixes.UnusedOpens
            let! sourceText = document.GetTextAsync()
            let! parseResults, _, checkResults = checker.ParseAndCheckDocument(document, options, userOpName = userOpName)
            let! unusedOpens = UnusedOpens.getUnusedOpens(checkResults, fun lineNumber -> sourceText.Lines.[Line.toZ lineNumber].ToString()) |> liftAsync
            return parseResults, checkResults, unusedOpens
        } 

    interface IFSharpUnusedOpensDiagnosticAnalyzer with

        member _.AnalyzeSemanticsAsync(descriptor, document: Document, cancellationToken: CancellationToken) =
            asyncMaybe {
                do Trace.TraceInformation("{0:n3} (start) UnusedOpensAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
                let! _parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync()
                let checker = checkerProvider.Checker
                let! parseResults, checkResults, unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges(document, projectOptions, checker)
            
                // We run analyzers as part of unused opens to give lower priority
                let! analyzerDiagnostics = checker.AnalyzeFileInProject(parseResults, checkResults, projectOptions, userOpName=userOpName) |> liftAsync
                let analyzerDiagnostics = FSharpDocumentDiagnosticAnalyzer.CleanDiagnostics(document.FilePath, analyzerDiagnostics, sourceText)
                return 
                    unusedOpens
                    |> List.map (fun range ->
                          Diagnostic.Create(
                             descriptor,
                             RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)))
                    |> Seq.toImmutableArray
                    |> fun a -> a.AddRange(analyzerDiagnostics)
            } 
            |> Async.map (Option.defaultValue ImmutableArray.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
