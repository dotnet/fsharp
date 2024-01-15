// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.Diagnostics
open CancellableTasks
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

[<Struct; NoComparison; NoEquality; RequireQualifiedAccess>]
type internal DiagnosticsType =
    | Syntax
    | Semantic

[<Export(typeof<IFSharpDocumentDiagnosticAnalyzer>)>]
type internal FSharpDocumentDiagnosticAnalyzer [<ImportingConstructor>] () =

    static let diagnosticEqualityComparer =
        { new IEqualityComparer<FSharpDiagnostic> with

            member _.Equals(x, y) =
                x.FileName = y.FileName
                && x.StartLine = y.StartLine
                && x.EndLine = y.EndLine
                && x.StartColumn = y.StartColumn
                && x.EndColumn = y.EndColumn
                && x.Severity = y.Severity
                && x.Message = y.Message
                && x.Subcategory = y.Subcategory
                && x.ErrorNumber = y.ErrorNumber

            member _.GetHashCode x =
                let mutable hash = 17
                hash <- hash * 23 + x.StartLine.GetHashCode()
                hash <- hash * 23 + x.EndLine.GetHashCode()
                hash <- hash * 23 + x.StartColumn.GetHashCode()
                hash <- hash * 23 + x.EndColumn.GetHashCode()
                hash <- hash * 23 + x.Severity.GetHashCode()
                hash <- hash * 23 + x.Message.GetHashCode()
                hash <- hash * 23 + x.Subcategory.GetHashCode()
                hash <- hash * 23 + x.ErrorNumber.GetHashCode()
                hash
        }

    static member GetDiagnostics(document: Document, diagnosticType: DiagnosticsType) =
        cancellableTask {

            let eventProps: (string * obj) array =
                [|
                    "context.document.project.id", document.Project.Id.Id.ToString()
                    "context.document.id", document.Id.Id.ToString()
                    "context.diagnostics.type",
                    match diagnosticType with
                    | DiagnosticsType.Syntax -> "syntax"
                    | DiagnosticsType.Semantic -> "semantic"
                |]

            use _eventDuration =
                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.GetDiagnosticsForDocument, eventProps)

            let! ct = CancellableTask.getCancellationToken ()

            let! sourceText = document.GetTextAsync(ct)
            let filePath = document.FilePath

            let errors = HashSet<FSharpDiagnostic>(diagnosticEqualityComparer)

            let! parseResults = document.GetFSharpParseResultsAsync("GetDiagnostics")

            // Old logic, rollback once https://github.com/dotnet/fsharp/issues/15972 is fixed (likely on Roslyn side, since we're returning diagnostics, but they're not getting to VS).
            (*
            match diagnosticType with
            | DiagnosticsType.Syntax ->
                for diagnostic in parseResults.Diagnostics do
                    errors.Add(diagnostic) |> ignore

            | DiagnosticsType.Semantic ->
                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync("GetDiagnostics")

                for diagnostic in checkResults.Diagnostics do
                    errors.Add(diagnostic) |> ignore

                errors.ExceptWith(parseResults.Diagnostics)
            *)

            // TODO: see comment above, this is a workaround for issue we have in current VS/Roslyn
            match diagnosticType with
            | DiagnosticsType.Syntax ->
                for diagnostic in parseResults.Diagnostics do
                    errors.Add(diagnostic) |> ignore

            // We always add syntactic, and do not exclude them when semantic is requested
            | DiagnosticsType.Semantic ->
                for diagnostic in parseResults.Diagnostics do
                    errors.Add(diagnostic) |> ignore

                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync("GetDiagnostics")

                for diagnostic in checkResults.Diagnostics do
                    errors.Add(diagnostic) |> ignore

            let! unnecessaryParentheses =
                match diagnosticType with
                | DiagnosticsType.Syntax when document.Project.IsFsharpRemoveParensEnabled ->
                    UnnecessaryParenthesesDiagnosticAnalyzer.GetDiagnostics document
                | _ -> CancellableTask.singleton ImmutableArray.Empty

            if errors.Count = 0 && unnecessaryParentheses.IsEmpty then
                return ImmutableArray.Empty
            else
                let iab = ImmutableArray.CreateBuilder(errors.Count + unnecessaryParentheses.Length)

                for diagnostic in errors do
                    if diagnostic.StartLine <> 0 && diagnostic.EndLine <> 0 then
                        let linePositionSpan =
                            LinePositionSpan(
                                LinePosition(diagnostic.StartLine - 1, diagnostic.StartColumn),
                                LinePosition(diagnostic.EndLine - 1, diagnostic.EndColumn)
                            )

                        let textSpan = sourceText.Lines.GetTextSpan(linePositionSpan)

                        // F# compiler report errors at end of file if parsing fails. It should be corrected to match Roslyn boundaries
                        let correctedTextSpan =
                            if textSpan.End <= sourceText.Length then
                                textSpan
                            else
                                let start = min textSpan.Start (sourceText.Length - 1) |> max 0

                                TextSpan.FromBounds(start, sourceText.Length)

                        let location = Location.Create(filePath, correctedTextSpan, linePositionSpan)
                        iab.Add(RoslynHelpers.ConvertError(diagnostic, location))

                iab.AddRange unnecessaryParentheses
                return iab.ToImmutable()
        }

    interface IFSharpDocumentDiagnosticAnalyzer with

        member _.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) : Task<ImmutableArray<Diagnostic>> =
            if document.Project.IsFSharpMetadata then
                Task.FromResult ImmutableArray.Empty
            else
                FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Syntax)
                |> CancellableTask.start cancellationToken

        member _.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) : Task<ImmutableArray<Diagnostic>> =
            if document.Project.IsFSharpMiscellaneousOrMetadata && not document.IsFSharpScript then
                Task.FromResult ImmutableArray.Empty
            else
                FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Semantic)
                |> CancellableTask.start cancellationToken
