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

type internal SendToInteractiveKind =
    | Selection
    | Line

    member kind.Status =
        match kind with
        | Selection -> OLECMDF.OLECMDF_SUPPORTED ||| OLECMDF.OLECMDF_ENABLED
        | Line ->
            OLECMDF.OLECMDF_SUPPORTED
            ||| OLECMDF.OLECMDF_ENABLED
            ||| OLECMDF.OLECMDF_DEFHIDEONCTXTMENU

[<AutoOpen>]
module private InteractiveCommand =

    [<return: Struct>]
    let (|SendToInteractive|_|) (group: Guid, commandId: uint32) =
        match group = VSConstants.VsStd11, enum<VSConstants.VSStd11CmdID> (int commandId) with
        | true, VSConstants.VSStd11CmdID.ExecuteSelectionInInteractive -> ValueSome Selection
        | true, VSConstants.VSStd11CmdID.ExecuteLineInInteractive -> ValueSome Line
        | _ -> ValueNone

type internal EditorSubmission =
    {
        Text: string
        SourcePath: string

        /// One-based, as the compiler numbers lines.
        StartLine: int
    }

module internal EditorSubmission =

    /// Sending a line also advances the caret, which is what makes repeated Alt+Enter walk down a script.
    let read (documentFactory: ITextDocumentFactoryService) (view: ITextView) (kind: SendToInteractiveKind) =
        let selection = view.Selection
        let sendsLine = kind.IsLine || selection.IsEmpty

        let span =
            if sendsLine then
                view.Caret.Position.BufferPosition.GetContainingLine().Extent
            else
                SnapshotSpan(selection.Start.Position, selection.End.Position)

        let text = span.GetText()

        if String.IsNullOrWhiteSpace text then
            ValueNone
        else
            let sourcePath =
                match documentFactory.TryGetTextDocument view.TextBuffer with
                | true, document -> document.FilePath
                | _ -> ""

            let snapshot = view.TextSnapshot
            let line = span.Start.GetContainingLine().LineNumber

            if sendsLine && line + 1 < snapshot.LineCount then
                view.Caret.MoveTo(snapshot.GetLineFromLineNumber(line + 1).Start) |> ignore
                selection.Clear()

            ValueSome
                {
                    Text = text
                    SourcePath = sourcePath
                    StartLine = line + 1
                }

type internal FSharpInteractiveCommandFilter
    (provider: FSharpVsInteractiveWindowProvider, documentFactory: ITextDocumentFactoryService, view: ITextView) =

    let mutable nextTarget: IOleCommandTarget | null = null

    member this.AttachToViewAdapter(viewAdapter: IVsTextView) =
        match viewAdapter.AddCommandFilter this with
        | VSConstants.S_OK, next -> nextTarget <- next
        | errorCode, _ -> ErrorHandler.ThrowOnFailure errorCode |> ignore

    interface IOleCommandTarget with

        member _.Exec(pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut) =
            match pguidCmdGroup, nCmdId with
            | SendToInteractive kind ->
                EditorSubmission.read documentFactory view kind
                |> ValueOption.iter (fun submission ->
                    provider.SubmitFromEditor(submission.Text, submission.SourcePath, submission.StartLine))

                VSConstants.S_OK
            | _ ->
                match nextTarget with
                | null -> VSConstants.E_FAIL
                | target -> target.Exec(&pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut)

        member _.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText) =
            let group = pguidCmdGroup

            let statuses =
                Array.init (int cCmds) (fun i ->
                    match group, prgCmds[i].cmdID with
                    | SendToInteractive kind -> ValueSome kind.Status
                    | _ -> ValueNone)

            let result =
                if statuses.Length > 0 && statuses |> Array.forall _.IsSome then
                    VSConstants.S_OK
                else
                    match nextTarget with
                    | null -> VSConstants.E_FAIL
                    | target -> target.QueryStatus(&pguidCmdGroup, cCmds, prgCmds, pCmdText)

            statuses
            |> Array.iteri (fun i status -> status |> ValueOption.iter (fun flags -> prgCmds[i].cmdf <- uint32 flags))

            result

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
