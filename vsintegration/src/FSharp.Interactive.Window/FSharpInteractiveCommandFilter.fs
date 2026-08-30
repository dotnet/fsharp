// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.ComponentModel.Composition

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities

/// What the user asked to send.
type internal SendToInteractiveKind =
    | Selection
    | Line

/// The shell delivers both send-to-interactive commands in the same group, so recognising one
/// means checking the group and the identifier together.
[<AutoOpen>]
module private InteractiveCommand =

    let (|SendSelection|SendLine|NotOurs|) (group: Guid, commandId: uint32) =
        if group <> VSConstants.VsStd11 then NotOurs
        elif commandId = uint32 VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive then SendSelection
        elif commandId = uint32 VSConstants.VSStd11CmdID.ExecuteLineInInteractive then SendLine
        else NotOurs

/// The text an editor command sends, and where it came from.
type internal EditorSubmission =
    {
        Text: string

        /// The file the text came from, so that the session reports diagnostics against it.
        SourcePath: string

        /// One-based, matching the line numbering the compiler reports.
        StartLine: int
    }

module internal EditorSubmission =

    let private lineOfCaret (view: ITextView) =
        view.Caret.Position.BufferPosition.GetContainingLine()

    /// The selection, or the caret's line when there is none. Sending a line also advances the
    /// caret, which is what makes repeated Alt+Enter walk down a script.
    let read (documentFactory: ITextDocumentFactoryService) (view: ITextView) kind =
        let selection = view.Selection

        let span, advanceCaret =
            match kind with
            | Line -> (lineOfCaret view).Extent, true
            | Selection when selection.IsEmpty -> (lineOfCaret view).Extent, true
            | Selection -> SnapshotSpan(selection.Start.Position, selection.End.Position), false

        let text = span.GetText()

        if String.IsNullOrWhiteSpace text then
            ValueNone
        else
            let sourcePath =
                match documentFactory.TryGetTextDocument view.TextBuffer with
                | true, document -> document.FilePath
                | _ -> ""

            if advanceCaret then
                let snapshot = view.TextSnapshot
                let next = span.Start.GetContainingLine().LineNumber + 1

                if next < snapshot.LineCount then
                    let start = snapshot.GetLineFromLineNumber(next).Start
                    view.Caret.MoveTo start |> ignore
                    view.Selection.Clear()

            ValueSome
                {
                    Text = text
                    SourcePath = sourcePath
                    StartLine = span.Start.GetContainingLine().LineNumber + 1
                }

/// Routes the editor's send-to-interactive commands to the F# Interactive window.
type internal FSharpInteractiveCommandFilter
    (provider: FSharpVsInteractiveWindowProvider, documentFactory: ITextDocumentFactoryService, view: ITextView) as this =

    let mutable nextTarget: IOleCommandTarget | null = null

    let send kind =
        match EditorSubmission.read documentFactory view kind with
        | ValueNone -> ()
        | ValueSome submission -> provider.SubmitFromEditor(submission.Text, submission.SourcePath, submission.StartLine)

    member _.AttachToViewAdapter(viewAdapter: IVsTextView) =
        match viewAdapter.AddCommandFilter this with
        | VSConstants.S_OK, next -> nextTarget <- next
        | errorCode, _ -> ErrorHandler.ThrowOnFailure errorCode |> ignore

    interface IOleCommandTarget with

        member _.Exec(pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) =
            match pguidCmdGroup, nCmdId with
            | SendSelection ->
                send Selection
                VSConstants.S_OK
            | SendLine ->
                send Line
                VSConstants.S_OK
            | NotOurs ->
                match nextTarget with
                | null -> VSConstants.E_FAIL
                | target -> target.Exec(&pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut)

        member _.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText) =
            match pguidCmdGroup with
            | group when group = VSConstants.VsStd11 ->
                for i in 0 .. int cCmds - 1 do
                    match group, prgCmds[i].cmdID with
                    | SendSelection -> prgCmds[i].cmdf <- uint32 (OLECMDF.OLECMDF_SUPPORTED ||| OLECMDF.OLECMDF_ENABLED)
                    | SendLine ->
                        prgCmds[i].cmdf <-
                            uint32 (
                                OLECMDF.OLECMDF_SUPPORTED
                                ||| OLECMDF.OLECMDF_ENABLED
                                ||| OLECMDF.OLECMDF_DEFHIDEONCTXTMENU
                            )
                    | NotOurs -> ()

                VSConstants.S_OK
            | _ ->
                match nextTarget with
                | null -> VSConstants.E_FAIL
                | target -> target.QueryStatus(&pguidCmdGroup, cCmds, prgCmds, pCmdText)

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(InteractiveWindowGuids.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.PrimaryDocument)>]
type internal FSharpInteractiveCommandFilterProvider
    [<ImportingConstructor>]
    (
        provider: FSharpVsInteractiveWindowProvider,
        documentFactory: ITextDocumentFactoryService,
        editorFactory: IVsEditorAdaptersFactoryService
    ) =

    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(view) =
            match editorFactory.GetViewAdapter view with
            | null -> ()
            | adapter -> FSharpInteractiveCommandFilter(provider, documentFactory, view).AttachToViewAdapter adapter
