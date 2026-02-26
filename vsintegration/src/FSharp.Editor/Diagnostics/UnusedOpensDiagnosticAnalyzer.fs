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
open Internal.Utilities.Library

[<Export(typeof<IFSharpUnusedOpensDiagnosticAnalyzer>)>]
type internal UnusedOpensDiagnosticAnalyzer [<ImportingConstructor>] () =

    static member GetUnusedOpenRanges(document: Document) =
        async2 {
            if not document.Project.IsFSharpCodeFixesUnusedOpensEnabled then
                return ValueNone
            else
                let! ct = Async2.CancellationToken
                let! sourceText = document.GetTextAsync ct

                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof UnusedOpensDiagnosticAnalyzer)

                let! unusedOpens =
                    UnusedOpens.getUnusedOpens (checkResults, (fun lineNumber -> sourceText.Lines[Line.toZ lineNumber].ToString()))

                return (ValueSome unusedOpens)
        }

    interface IFSharpUnusedOpensDiagnosticAnalyzer with

        member _.AnalyzeSemanticsAsync(descriptor, document: Document, cancellationToken: CancellationToken) =
            if document.Project.IsFSharpMiscellaneousOrMetadata && not document.IsFSharpScript then
                Tasks.Task.FromResult(ImmutableArray.Empty)
            else
                async2 {
                    do Trace.TraceInformation("{0:n3} (start) UnusedOpensAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
                    let! sourceText = document.GetTextAsync()
                    let! unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges document

                    return
                        unusedOpens
                        |> ValueOption.defaultValue List.Empty
                        |> List.map (fun range ->
                            Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)))
                        |> Seq.toImmutableArray
                }
                |> Async2.startInThreadPool cancellationToken
