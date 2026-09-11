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

    // A reference, not a voption: the reactor reads it while the UI thread writes, and must never see a torn struct.
    [<VolatileField>]
    let mutable position: Position option = None

    let lineChanged = Event<unit>()

    /// None while no editor on the buffer has focus.
    member _.Position = position

    /// Raised on the UI thread when the caret moves to another line, or focus enters or leaves the buffer's editors.
    member _.LineChanged = lineChanged.Publish

    member _.Update(newPosition: Position option) =
        let hasLineChanged = Option.map _.Line position <> Option.map _.Line newPosition
        position <- newPosition

        if hasLineChanged then
            lineChanged.Trigger()

    static member TryGet(sourceText: SourceText) =
        match sourceText.Container.TryGetTextBuffer() with
        | null -> ValueNone
        | buffer ->
            match buffer.Properties.TryGetProperty<FocusedCaret>(typeof<FocusedCaret>) with
            | true, caret -> ValueSome caret
            | _ -> ValueNone

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
            let focusedCaret =
                textView.TextBuffer.Properties.GetOrCreateSingletonProperty(fun () -> FocusedCaret())

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
                    subscription.Dispose())
