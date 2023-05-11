// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Immutable
open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open CancellableTasks
open System.Threading.Tasks

// So the Roslyn interface is called IFSharpInlineHintsService
// but our implementation is called just HintsService.
// That's because we'll likely use this API for things other than inline hints,
// e.g. signature hints above the line, pipeline hints on the side and so on.

[<Export(typeof<IFSharpInlineHintsService>)>]
type internal FSharpInlineHintsService [<ImportingConstructor>] (settings: EditorOptions) =

    static let userOpName = "Hints"

    interface IFSharpInlineHintsService with
        member _.GetInlineHintsAsync(document, _, cancellationToken) =
            let hintKinds = OptionParser.getHintKinds settings.Advanced

            if hintKinds.IsEmpty then
                Task.FromResult ImmutableArray.Empty
            else
                cancellableTask {
                    let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                    let hintKindsSerialized = hintKinds |> Set.map Hints.serialize |> String.concat ","
                    TelemetryReporter.reportEvent "hints" [ ("hints.kinds", hintKindsSerialized) ]

                    let! sourceText = document.GetTextAsync cancellationToken
                    let! nativeHints = HintService.getHintsForDocument sourceText document hintKinds userOpName

                    let tasks =
                        nativeHints
                        |> Seq.map (fun hint -> NativeToRoslynHintConverter.convert sourceText hint cancellationToken)

                    let! roslynHints = Task.WhenAll(tasks)

                    return roslynHints.ToImmutableArray()
                }
                |> CancellableTask.start cancellationToken
