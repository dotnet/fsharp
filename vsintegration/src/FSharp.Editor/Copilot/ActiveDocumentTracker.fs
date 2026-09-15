// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

/// The file of the editor that last took focus, and the lines its caret or selection covers - 1-based
/// and inclusive, as the parse tree counts them.
type internal EditorFocus =
    {
        FilePath: string
        FirstLine: int
        LastLine: int
    }

/// Written from the UI thread as focus, caret and selection move, read from wherever a brokered service
/// happens to run; the lock keeps a read from pairing one file with another file's lines. A read that
/// races a tab switch names the tab before it, which costs a mention its place in a picker and nothing else.
[<Export>]
[<PartCreationPolicy(CreationPolicy.Shared)>]
type internal FSharpActiveDocumentTracker() =

    let gate = obj ()
    let mutable focus = ValueNone

    member _.Focus = lock gate (fun () -> focus)

    member _.SetFocus value =
        lock gate (fun () -> focus <- ValueSome value)

/// Every content type, not just F#: a C# file taking focus has to displace the F# one, or a declaration
/// would answer as focused while its file is off screen.
[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType("text")>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal FSharpActiveDocumentListener
    [<ImportingConstructor>]
    (tracker: FSharpActiveDocumentTracker, textDocumentFactory: ITextDocumentFactoryService) =

    static let lineOf (point: SnapshotPoint) =
        point.GetContainingLine().LineNumber + 1

    /// A selection ends before its End point: one of whole lines ends at the start of the line after them.
    static let linesOf (textView: ITextView) =
        let selection = textView.Selection

        if selection.IsEmpty then
            let caret = lineOf textView.Caret.Position.BufferPosition
            struct (caret, caret)
        else
            let firstLine = lineOf selection.Start.Position
            struct (firstLine, max firstLine (lineOf (selection.End.Position - 1)))

    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(textView: IWpfTextView) =
            let recordFocus () =
                match textDocumentFactory.TryGetTextDocument textView.TextBuffer with
                | true, document ->
                    let struct (firstLine, lastLine) = linesOf textView

                    tracker.SetFocus
                        {
                            FilePath = document.FilePath
                            FirstLine = firstLine
                            LastLine = lastLine
                        }
                | _ -> ()

            let recordWhileFocused () =
                if textView.HasAggregateFocus then
                    recordFocus ()

            textView.GotAggregateFocus.Add(fun _ -> recordFocus ())
            textView.Caret.PositionChanged.Add(fun _ -> recordWhileFocused ())
            textView.Selection.SelectionChanged.Add(fun _ -> recordWhileFocused ())
            recordWhileFocused ()
