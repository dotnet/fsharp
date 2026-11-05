// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

open FSharp.Compiler.Text

/// The caret of the focused editor, published by the UI thread so the options reactor never has to wait for it.
[<Sealed>]
type internal FocusedCaret() =

    // option, not voption: a struct read racing the UI thread's write could tear.
    [<VolatileField>]
    let mutable position: Position option = None

    let lineChanged = Event<unit>()

    member _.Position = position

    member _.LineChanged = lineChanged.Publish

    member _.Update(newPosition: Position option) =
        let hasLineChanged =
            match position, newPosition with
            | Some previous, Some current -> previous.Line <> current.Line
            | None, None -> false
            | _ -> true

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
                    // Edits move the caret without raising PositionChanged.
                    textView.LayoutChanged.Subscribe(fun args ->
                        if args.NewSnapshot <> args.OldSnapshot && textView.HasAggregateFocus then
                            publish ())
                    textView.GotAggregateFocus.Subscribe publish
                    textView.LostAggregateFocus.Subscribe(fun _ -> focusedCaret.Update None)
                ]

            if textView.HasAggregateFocus then
                publish ()

            textView.Closed.Add(fun _ ->
                for subscription in subscriptions do
                    subscription.Dispose()

                // The caret belongs to the buffer, which outlives this view.
                if textView.HasAggregateFocus then
                    focusedCaret.Update None)
