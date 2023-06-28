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

[<RequireQualifiedAccess>]
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

            let! ct = CancellableTask.getCurrentCancellationToken ()

            let! parseResults = document.GetFSharpParseResultsAsync("GetDiagnostics")

            let! sourceText = document.GetTextAsync(ct)
            let filePath = document.FilePath

            let! errors =
                cancellableTask {
                    match diagnosticType with
                    | DiagnosticsType.Semantic ->
                        let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync("GetDiagnostics")
                        // In order to eliminate duplicates, we should not return parse errors here because they are returned by `AnalyzeSyntaxAsync` method.
                        let allDiagnostics = HashSet(checkResults.Diagnostics, diagnosticEqualityComparer)
                        allDiagnostics.ExceptWith(parseResults.Diagnostics)
                        return Seq.toArray allDiagnostics
                    | DiagnosticsType.Syntax -> return parseResults.Diagnostics
                }

            let results =
                HashSet(errors, diagnosticEqualityComparer)
                |> Seq.choose (fun diagnostic ->
                    if diagnostic.StartLine = 0 || diagnostic.EndLine = 0 then
                        // F# diagnostic line numbers are one-based. Compiler returns 0 for global errors (reported by ProjectDiagnosticAnalyzer)
                        None
                    else
                        // Roslyn line numbers are zero-based
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
                        Some(RoslynHelpers.ConvertError(diagnostic, location)))
                |> Seq.toImmutableArray

            return results
        }

    interface IFSharpDocumentDiagnosticAnalyzer with

        member _.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) : Task<ImmutableArray<Diagnostic>> =
            cancellableTask {
                if document.Project.IsFSharpMetadata then
                    return ImmutableArray.Empty
                else
                    return! FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Syntax)
            }
            |> CancellableTask.start cancellationToken

        member _.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) : Task<ImmutableArray<Diagnostic>> =
            cancellableTask {
                if document.Project.IsFSharpMiscellaneousOrMetadata && not document.IsFSharpScript then
                    return ImmutableArray.Empty
                else
                    return! FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Semantic)
            }
            |> CancellableTask.start cancellationToken
