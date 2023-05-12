// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Immutable
open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

// So the Roslyn interface is called IFSharpInlineHintsService
// but our implementation is called just HintsService.
// That's because we'll likely use this API for things other than inline hints,
// e.g. signature hints above the line, pipeline hints on the side and so on.

[<Export(typeof<IFSharpInlineHintsService>)>]
type internal RoslynAdapter [<ImportingConstructor>] (settings: EditorOptions) =

    static let userOpName = "Hints"

    interface IFSharpInlineHintsService with
        member _.GetInlineHintsAsync(document, _, cancellationToken) =
            async {
                let hintKinds = OptionParser.getHintKinds settings.Advanced

                if hintKinds.IsEmpty then
                    return ImmutableArray.Empty
                else
                    let hintKindsSerialized = hintKinds |> Set.map Hints.serialize |> String.concat ","
                    TelemetryReporter.ReportSingleEvent ("hints", [| ("hints.kinds", hintKindsSerialized) |])

                    let! sourceText = document.GetTextAsync cancellationToken |> Async.AwaitTask
                    let! nativeHints = HintService.getHintsForDocument sourceText document hintKinds userOpName cancellationToken

                    let roslynHints =
                        nativeHints |> Seq.map (NativeToRoslynHintConverter.convert sourceText)

                    return roslynHints.ToImmutableArray()
            }
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
