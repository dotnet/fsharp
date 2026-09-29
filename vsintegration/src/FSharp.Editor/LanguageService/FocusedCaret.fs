// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

open FSharp.Compiler.Text

/// The caret of the focused editor on a text buffer, published by the UI thread for the project options
/// reactor: the UI thread can be blocked waiting for the reactor, so the reactor must never wait for it.
[<Sealed>]
type internal FocusedCaret() =

    // A reference, not an option: the reactor reads it while the UI thread writes, and must never see a torn struct.
    [<VolatileField>]
    let mutable position: Position option = None

    let lineChanged = Event<unit>()

    /// None while no editor on the buffer has focus.
    member _.Position = position

    /// Raised on the UI thread when the caret moves to another line, or focus enters or leaves the buffer's editors.
    member _.LineChanged = lineChanged.Publish

    member _.Update(newPosition: Position option) =
        let hasLineChanged =
            (position |> Option.map _.Line) <> (newPosition |> Option.map _.Line)

        position <- newPosition

        if hasLineChanged then
            lineChanged.Trigger()

    /// The one caret of a buffer, created by whichever asks first: the reactor can compute a script's options
    /// before any editor on it exists, and must already hold the caret that editor will publish to.
    /// The property collection is synchronized, so this is safe off the UI thread.
    static member Of(properties: PropertyCollection) =
        properties.GetOrCreateSingletonProperty(fun () -> FocusedCaret())

    /// None for a text no buffer holds, which no editor can show.
    static member TryGetOrCreate(sourceText: SourceText) =
        match sourceText.Container.TryGetTextBuffer() with
        | null -> ValueNone
        | buffer -> ValueSome(FocusedCaret.Of buffer.Properties)

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.Editable)>]
type internal FocusedCaretTracker() =

    let caretOf (textView: ITextView) =
        let caret = textView.Caret.Position.BufferPosition
        let line = caret.GetContainingLine()
        Position.fromZ line.LineNumber (caret.Position - line.Start.Position)

    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(textView) =
            let focusedCaret = FocusedCaret.Of textView.TextBuffer.Properties

            let publish _ =
                focusedCaret.Update(Some(caretOf textView))

            let subscriptions =
                [
                    textView.Caret.PositionChanged.Subscribe(fun _ ->
                        if textView.HasAggregateFocus then
                            publish ())
                    textView.GotAggregateFocus.Subscribe publish
                    textView.LostAggregateFocus.Subscribe(fun _ -> focusedCaret.Update None)
                ]

            if textView.HasAggregateFocus then
                publish ()

            textView.Closed.Add(fun _ ->
                for subscription in subscriptions do
                    subscription.Dispose()

                // The caret belongs to the buffer, which outlives the view. A view closed while it held focus
                // would leave its line published, and the reactor would go on skipping the `#r` on a line no
                // editor is on. A view that does not hold focus published nothing to clear.
                if textView.HasAggregateFocus then
                    focusedCaret.Update None)
