// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Immutable
open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open Internal.Utilities.Library
open System.Threading.Tasks

// So the Roslyn interface is called IFSharpInlineHintsService
// but our implementation is called just HintsService.
// That's because we'll likely use this API for things other than inlay hints,
// e.g. signature hints above the line, pipeline hints on the side and so on.

[<Export(typeof<IFSharpInlineHintsService>)>]
type internal FSharpInlayHintsService [<ImportingConstructor>] (settings: EditorOptions) =

    static let userOpName = "Hints"

    interface IFSharpInlineHintsService with
        member _.GetInlineHintsAsync(document, _, cancellationToken) =
            let hintKinds = OptionParser.getHintKinds settings.Advanced

            if hintKinds.IsEmpty then
                Task.FromResult ImmutableArray.Empty
            else
                async2 {
                    let! cancellationToken = Async2.CancellationToken

                    let! sourceText = document.GetTextAsync cancellationToken
                    let! nativeHints = HintService.getHintsForDocument sourceText document hintKinds userOpName

                    let tasks =
                        nativeHints
                        |> Seq.map (fun hint -> NativeToRoslynHintConverter.convert sourceText hint)

                    let! roslynHints = tasks |> Async2.Parallel

                    return roslynHints.ToImmutableArray()
                }
                |> Async2.startInThreadPool cancellationToken
