// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Diagnostics
open System.Threading

open Microsoft.CodeAnalysis

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics
open CancellableTasks

[<Export(typeof<IFSharpUnusedOpensDiagnosticAnalyzer>)>]
type internal UnusedOpensDiagnosticAnalyzer [<ImportingConstructor>] () =

    static member GetUnusedOpenRanges(document: Document) =
        cancellableTask {
            if not document.Project.IsFSharpCodeFixesUnusedOpensEnabled then
                Trace.TraceInformation(
                    "[UnusedOpensAnalyzer] GetUnusedOpenRanges: SKIPPED (IsFSharpCodeFixesUnusedOpensEnabled=false) doc={0} proj={1}",
                    document.Name,
                    document.Project.Name
                )

                return ValueNone
            else
                let! ct = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync ct

                Trace.TraceInformation(
                    "[UnusedOpensAnalyzer] GetUnusedOpenRanges: getting parse/check results for doc={0} proj={1} sourceLen={2}",
                    document.Name,
                    document.Project.Name,
                    sourceText.Length
                )

                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof UnusedOpensDiagnosticAnalyzer)

                Trace.TraceInformation(
                    "[UnusedOpensAnalyzer] GetUnusedOpenRanges: parse/check complete. HasErrors={0} Diagnostics={1}",
                    checkResults.HasErrors,
                    checkResults.Diagnostics.Length
                )

                if checkResults.HasErrors then
                    Trace.TraceInformation("[UnusedOpensAnalyzer] GetUnusedOpenRanges: SKIPPED (checkResults.HasErrors=true)")
                    return ValueNone
                else
                    let! unusedOpens =
                        UnusedOpens.getUnusedOpens (checkResults, (fun lineNumber -> sourceText.Lines[Line.toZ lineNumber].ToString()))

                    Trace.TraceInformation(
                        "[UnusedOpensAnalyzer] GetUnusedOpenRanges: getUnusedOpens returned {0} ranges",
                        unusedOpens.Length
                    )

                    return (ValueSome unusedOpens)
        }

    interface IFSharpUnusedOpensDiagnosticAnalyzer with

        member _.AnalyzeSemanticsAsync(descriptor, document: Document, cancellationToken: CancellationToken) =
            Trace.TraceInformation(
                "[UnusedOpensAnalyzer] AnalyzeSemanticsAsync ENTER: doc={0} proj={1} isMisc={2} isScript={3}",
                document.Name,
                document.Project.Name,
                document.Project.IsFSharpMiscellaneousOrMetadata,
                document.IsFSharpScript
            )

            if document.Project.IsFSharpMiscellaneousOrMetadata && not document.IsFSharpScript then
                Trace.TraceInformation("[UnusedOpensAnalyzer] AnalyzeSemanticsAsync: SKIPPED (misc/metadata non-script)")
                Tasks.Task.FromResult(ImmutableArray.Empty)
            else
                cancellableTask {
                    do Trace.TraceInformation("{0:n3} (start) UnusedOpensAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
                    let! sourceText = document.GetTextAsync()
                    let! unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges document

                    let result =
                        unusedOpens
                        |> ValueOption.defaultValue List.Empty
                        |> List.map (fun range ->
                            Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)))
                        |> Seq.toImmutableArray

                    Trace.TraceInformation(
                        "[UnusedOpensAnalyzer] AnalyzeSemanticsAsync: returning {0} diagnostics for doc={1}",
                        result.Length,
                        document.Name
                    )

                    return result
                }
                |> CancellableTask.start cancellationToken
