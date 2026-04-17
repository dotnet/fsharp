// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Immutable
open System.ComponentModel.Composition
open System.Threading.Tasks
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints
open Microsoft.VisualStudio.FSharp.Editor
open CancellableTasks

// So the Roslyn interface is called IFSharpInlineHintsService
// but our implementation is called just HintsService.
// That's because we'll likely use this API for things other than inlay hints,
// e.g. signature hints above the line, pipeline hints on the side and so on.

[<Export(typeof<IFSharpInlineHintsService2>)>]
type internal FSharpInlayHintsService [<ImportingConstructor>] (settings: EditorOptions) =

    static let userOpName = "Hints"

    interface IFSharpInlineHintsService2 with
        member _.GetInlineHintsAsync(document, textSpan, displayAllOverride, cancellationToken) =
            let hintKinds =
                if displayAllOverride then
                    Hints.allHintKinds
                else
                    OptionParser.getHintKinds settings.Advanced

            if hintKinds.IsEmpty then
                Task.FromResult ImmutableArray.Empty
            else
                cancellableTask {
                    let! cancellationToken = CancellableTask.getCancellationToken ()

                    let! sourceText = document.GetTextAsync cancellationToken
                    let! nativeHints = HintService.getHintsForDocument sourceText document hintKinds textSpan userOpName

                    let roslynHints =
                        nativeHints
                        |> Seq.map (NativeToRoslynHintConverter.convert sourceText)
                        |> ImmutableArray.CreateRange

                    return roslynHints
                }
                |> CancellableTask.start cancellationToken
