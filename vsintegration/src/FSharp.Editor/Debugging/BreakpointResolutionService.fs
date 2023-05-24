// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.Implementation.Debugging

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open CancellableTasks

[<Export(typeof<IFSharpBreakpointResolutionService>)>]
type internal FSharpBreakpointResolutionService [<ImportingConstructor>] () =

    static member GetBreakpointLocation(document: Document, textSpan: TextSpan) =
        cancellableTask {
            let! ct = CancellableTask.getCurrentCancellationToken ()

            let! sourceText = document.GetTextAsync(ct)

            let textLinePos = sourceText.Lines.GetLinePosition(textSpan.Start)

            let textInLine =
                sourceText.GetSubText(sourceText.Lines.[textLinePos.Line].Span).ToString()

            if String.IsNullOrWhiteSpace textInLine then
                return None
            else
                let textLineColumn = textLinePos.Character
                let fcsTextLineNumber = Line.fromZ textLinePos.Line // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based

                let! parseResults =
                    document.GetFSharpParseResultsAsync(nameof (FSharpBreakpointResolutionService))
                    |> liftAsync

                match parseResults with
                | Some parseResults -> return parseResults.ValidateBreakpointLocation(mkPos fcsTextLineNumber textLineColumn)
                | _ -> return None
        }

    interface IFSharpBreakpointResolutionService with
        member _.ResolveBreakpointAsync
            (
                document: Document,
                textSpan: TextSpan,
                cancellationToken: CancellationToken
            ) : Task<FSharpBreakpointResolutionResult> =
            cancellableTask {
                let! range = FSharpBreakpointResolutionService.GetBreakpointLocation(document, textSpan)
                match range with
                | None -> return Unchecked.defaultof<_>
                | Some range ->
                    let! sourceText = document.GetTextAsync(cancellationToken)
                    let span = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range)

                    match span with
                    | None -> return Unchecked.defaultof<_>
                    | Some span ->
                        return FSharpBreakpointResolutionResult.CreateSpanResult(document, span)
            }
            |> CancellableTask.start cancellationToken

        // FSROSLYNTODO: enable placing breakpoints by when user supplies fully-qualified function names
        member _.ResolveBreakpointsAsync(_, _, _) : Task<IEnumerable<FSharpBreakpointResolutionResult>> =
            Task.FromResult(Enumerable.Empty<FSharpBreakpointResolutionResult>())
