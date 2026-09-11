// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition

open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

/// The file whose editor last took focus. Written only from the UI thread, read without a lock from
/// wherever a brokered service happens to run: a read that races a tab switch names the tab before it,
/// which costs a mention its place in a picker and nothing else.
[<Export>]
[<PartCreationPolicy(CreationPolicy.Shared)>]
type internal FSharpActiveDocumentTracker() =

    let mutable focusedFilePath = ValueNone

    member _.FocusedFilePath = focusedFilePath

    member _.SetFocusedFilePath path = focusedFilePath <- ValueSome path

/// Every content type, not just F#: a C# file taking focus has to displace the F# one, or a declaration
/// would answer as focused while its file is off screen.
[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType("text")>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal FSharpActiveDocumentListener
    [<ImportingConstructor>]
    (tracker: FSharpActiveDocumentTracker, textDocumentFactory: ITextDocumentFactoryService) =

    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(textView: IWpfTextView) =
            let recordFocus () =
                match textDocumentFactory.TryGetTextDocument textView.TextBuffer with
                | true, document -> tracker.SetFocusedFilePath document.FilePath
                | _ -> ()

            textView.GotAggregateFocus.Add(fun _ -> recordFocus ())

            if textView.HasAggregateFocus then
                recordFocus ()
