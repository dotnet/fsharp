// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

// This implementation does not rely on Roslyn internals: everything Roslyn has for snippets lives in
// `Microsoft.VisualStudio.LanguageServices.Implementation.Snippets`, which is internal and has no
// ExternalAccess surface. Roslyn's `SnippetExpansionClient` is the design reference, not a base class.

open System
open System.Xml.Linq

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor.DebugHelpers
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager.Interop

open MSXML

[<AutoOpen>]
module internal SnippetExpansionHelpers =

    /// Measured on the snapshot itself: the caller only needs the width, and `GetText()` would copy the
    /// whole line to get it.
    let leadingWhitespaceOf (line: ITextSnapshotLine) =
        let snapshot = line.Snapshot
        let start = line.Start.Position
        let mutable width = 0

        while width < line.Length && Char.IsWhiteSpace snapshot[start + width] do
            width <- width + 1

        width

    /// Indentation spelled the way the document is configured to spell it, rather than the way this
    /// file happens to. F# registers `DefaultToInsertSpaces`, but the setting is the user's.
    let indentTextOf (options: IEditorOptions) width =
        if options.GetOptionValue DefaultOptions.ConvertTabsToSpacesOptionId then
            String(' ', width)
        else
            let tabSize = options.GetOptionValue DefaultOptions.TabSizeOptionId
            String('\t', width / tabSize) + String(' ', width % tabSize)

    /// Whether the line starts a directive wrapper, asking the snapshot for the one character that
    /// settles it before copying the line out to compare prefixes.
    let startsRootLevelDirective (line: ITextSnapshotLine) indent =
        line.Snapshot[line.Start.Position + indent] = '#'
        && SnippetIndentation.isRootLevelDirective (line.GetText())

    /// Where `$selected$` sits in a snippet's `<Code>`: which of its lines holds the field, and the
    /// column the template indents it to. That is the one nesting level a wrapper contributes, and the
    /// expansion session will not report it, so it is read from the file the picker named.
    let tryReadSelectedFieldLayout (path: string) =
        try
            XDocument.Load(path).Descendants()
            |> Seq.filter (fun element -> element.Name.LocalName = "Code")
            |> Seq.map _.Value
            |> Seq.tryHeadV
            |> ValueOption.bind (fun (code: string) ->
                code.Replace("\r\n", "\n").Split('\n')
                |> Seq.indexed
                |> Seq.tryPickV (fun (index, line: string) ->
                    match line.IndexOf("$selected$", StringComparison.Ordinal) with
                    | -1 -> ValueNone
                    | column -> ValueSome(index, column)))
        with e ->
            FSharpOutputPane.logException e
            ValueNone

    /// Splits `GenerateMatchCases($expression$)` into its name and its `$field$` arguments.
    let tryParseFunctionCall (call: string) =
        match call.IndexOf('(') with
        | -1 -> ValueNone
        | openParen when call.EndsWith(")", StringComparison.Ordinal) ->
            let name = call.Substring(0, openParen).Trim()

            let arguments =
                call.Substring(openParen + 1, call.Length - openParen - 2).Split(',')
                |> Array.map _.Trim()
                |> Array.filter (fun argument -> argument.Length > 0)

            if name.Length = 0 then
                ValueNone
            else
                ValueSome(name, arguments)
        | _ -> ValueNone

/// Everything Surround With needs to put the result back at the right column, none of which the
/// insertion can be asked for afterwards.
type internal SurroundLayout =
    {
        /// The column the wrapped code sat at.
        Column: int
        /// How many lines it covered.
        LineCount: int
        /// Which line of the template holds `$selected$`, and the column it indents it to.
        FieldLine: int
        FieldIndent: int
    }

/// Drives one snippet expansion in one text view. VS owns the session; this is the callback surface
/// it drives, plus the handful of operations the command handler needs.
type internal FSharpSnippetExpansionClient
    (textView: IWpfTextView, subjectBuffer: ITextBuffer, editorAdapters: IVsEditorAdaptersFactoryService) =

    let languageGuid = Guid FSharpConstants.languageServiceGuidString

    /// Set from `OnBeforeInsertion` rather than from `InsertNamedExpansion`'s out parameter: a snippet
    /// with no editable fields ends its session from inside that call, so the out parameter arrives
    /// after `EndExpansion` has already run.
    let mutable expansionSession: IVsExpansionSession = null

    /// Set when Surround With opens the picker, before the template is known.
    let mutable pendingSurround = ValueNone

    /// The same, completed with the chosen template's layout once the picker has answered. ValueNone
    /// for Insert Snippet, where the caret column is the whole answer.
    let mutable surround: SurroundLayout voption = ValueNone

    /// `FormatSpan` can be called more than once per session, and it inserts, so it must run once.
    let mutable indentPending = false

    member _.IsInSession =
        match expansionSession with
        | null -> false
        | _ -> true

    member private _.TryGetExpansion() =
        match editorAdapters.GetBufferAdapter subjectBuffer with
        | :? IVsExpansion as expansion -> ValueSome expansion
        | _ -> ValueNone

    /// Where the expansion goes: the caret, as an empty span.
    ///
    /// It must not be the selection. `tsInsertPos` is the range `InsertNamedExpansion` *replaces*, so
    /// handing it the selection deletes the text a SurroundsWith snippet was meant to wrap. The engine
    /// reads the selection off the view it was given in `InvokeInsertionUI` to fill `$selected$`, which
    /// is why the legacy `ExpansionProvider.OnItemChosen` passes `GetCaretPos` and nothing else.
    member private _.TryGetCaretSpan() =
        if not (obj.ReferenceEquals(textView.TextBuffer, subjectBuffer)) then
            // Nothing projects F# today; bail out rather than guess at a mapping.
            ValueNone
        else
            let caret = textView.Caret.Position.BufferPosition
            let line = caret.Snapshot.GetLineFromPosition caret.Position
            let column = caret.Position - line.Start.Position

            ValueSome(VsTextSpan(iStartLine = line.LineNumber, iStartIndex = column, iEndLine = line.LineNumber, iEndIndex = column))

    member private this.InsertNamedExpansion(title, path, insertionSpan: VsTextSpan) =
        match this.TryGetExpansion() with
        | ValueNone -> false
        | ValueSome expansion ->
            // The picker has named the template, so the field's place in it can be read now.
            surround <-
                match pendingSurround, tryReadSelectedFieldLayout path with
                | ValueSome(column, lineCount), ValueSome(fieldLine, fieldIndent) ->
                    ValueSome
                        {
                            Column = column
                            LineCount = lineCount
                            FieldLine = fieldLine
                            FieldIndent = fieldIndent
                        }
                | _ -> ValueNone

            indentPending <- true
            let mutable session = Unchecked.defaultof<IVsExpansionSession>

            let hr =
                expansion.InsertNamedExpansion(title, path, insertionSpan, this, languageGuid, 0, &session)

            not (ErrorHandler.Failed hr)

    /// Expands the snippet registered under `shortcut`, replacing `shortcutSpan`.
    member this.TryInsertExpansionForShortcut(shortcut: string, shortcutSpan: VsTextSpan) =
        match ServiceProvider.GlobalProvider.ExpansionManager, editorAdapters.GetViewAdapter textView with
        | null, _
        | _, null -> false
        | expansionManager, viewAdapter ->
            let spans = [| shortcutSpan |]
            let mutable path = null
            let mutable title = null

            let hr =
                expansionManager.GetExpansionByShortcut(this, languageGuid, shortcut, viewAdapter, spans, 0, &path, &title)

            if ErrorHandler.Failed hr then
                false
            else
                match path with
                | null -> false
                | path -> this.InsertNamedExpansion(title, path, spans[0])

    /// Shows a snippet picker. It is not modal: the chosen item comes back later through `OnItemChosen`.
    member private this.InvokeInsertionUI(types: string[], prompt) =
        match ServiceProvider.GlobalProvider.ExpansionManager, editorAdapters.GetViewAdapter textView with
        | null, _
        | _, null -> false
        | expansionManager, viewAdapter ->
            let hr =
                expansionManager.InvokeInsertionUI(viewAdapter, this, languageGuid, types, types.Length, 1, null, 0, 0, prompt, null)

            not (ErrorHandler.Failed hr)

    member this.TryInsertSnippet() =
        pendingSurround <- ValueNone
        surround <- ValueNone
        this.InvokeInsertionUI([| "Expansion"; "SurroundsWith" |], SR.InsertSnippet())

    /// `column` is where the selected code sits and `lineCount` how many lines it covers; neither
    /// survives the insertion, which replaces the selection.
    member this.TrySurroundWith(column: int, lineCount: int) =
        pendingSurround <- ValueSome(column, lineCount)
        this.InvokeInsertionUI([| "SurroundsWith" |], SR.SurroundWith())

    member private _.EndSession(leaveCaret) =
        match expansionSession with
        | null -> ()
        | session ->
            session.EndCurrentExpansion leaveCaret |> ignore
            expansionSession <- null

    member this.TryHandleTab() =
        match expansionSession with
        | null -> false
        | session ->
            // Navigation wraps around, so a failure means the session is no longer usable.
            if not (Com.Succeeded(session.GoToNextExpansionField 0)) then
                this.EndSession 0

            true

    member this.TryHandleBackTab() =
        match expansionSession with
        | null -> false
        | session ->
            if not (Com.Succeeded(session.GoToPreviousExpansionField())) then
                this.EndSession 0

            true

    member this.TryHandleReturn() =
        if this.IsInSession then
            this.EndSession 0
            true
        else
            false

    member this.TryHandleEscape() =
        if this.IsInSession then
            this.EndSession 1
            true
        else
            false

    interface IVsExpansionClient with

        member _.IsValidType(_buffer, _ts, _rgTypes, _iCountTypes, pfIsValidType: byref<int>) =
            pfIsValidType <- 1
            VSConstants.S_OK

        member _.IsValidKind(_buffer, _ts, _bstrKind, pfIsValidKind: byref<int>) =
            pfIsValidKind <- 1
            VSConstants.S_OK

        member _.OnBeforeInsertion(session) =
            expansionSession <- session
            VSConstants.S_OK

        member _.OnAfterInsertion _session = VSConstants.S_OK

        member _.PositionCaretForEditing(_buffer, _ts) = VSConstants.S_OK

        member _.EndExpansion() =
            expansionSession <- null
            pendingSurround <- ValueNone
            surround <- ValueNone
            VSConstants.S_OK

        member this.OnItemChosen(pszTitle, pszPath) =
            match this.TryGetCaretSpan() with
            | ValueSome span -> this.InsertNamedExpansion(pszTitle, pszPath, span) |> ignore
            | ValueNone -> ()

            VSConstants.S_OK

        member this.GetExpansionFunction(xmlFunctionNode: IXMLDOMNode, _bstrFieldName, pFunc: byref<IVsExpansionFunction>) =
            let getSession = fun () -> expansionSession

            match tryParseFunctionCall xmlFunctionNode.text with
            | ValueSome("ClassName", arguments) ->
                pFunc <- SnippetFunctionClassName(getSession, subjectBuffer, arguments)
                VSConstants.S_OK
            | ValueSome("GenerateMatchCases", arguments) ->
                pFunc <- SnippetFunctionGenerateMatchCases(getSession, subjectBuffer, arguments)
                VSConstants.S_OK
            | _ ->
                pFunc <- null
                VSConstants.E_INVALIDARG

        /// The expansion engine inserts the snippet verbatim: its first line lands at the insertion
        /// column, every later line at the column the template spells. F# has no formatter to reflow
        /// that, so the indentation is this method's job, and each kind of line wants a different one:
        ///
        ///  - a root-level directive (`#if`, `#endif`) belongs at column 0 whatever it wraps;
        ///  - text the engine substituted into `$selected$` already carries the indentation it had in
        ///    the buffer, and needs only the nesting the template adds around the field;
        ///  - every other line is the snippet's own, and takes the column of the code it wraps -
        ///    the caret's for Insert Snippet, the selection's for Surround With.
        member _.FormatSpan(_buffer, ts: VsTextSpan[]) =
            if indentPending && ts.Length > 0 then
                indentPending <- false
                let span = ts[0]
                let snapshot = subjectBuffer.CurrentSnapshot

                // `GetFieldSpan "selected"` does not answer for that special literal, so the range is
                // derived instead: the template says which of its lines holds the field and at what
                // column, and the command handler counted the lines the selection covered.
                let selectedLines =
                    match surround with
                    | ValueSome s -> ValueSome(span.iStartLine + s.FieldLine, span.iStartLine + s.FieldLine + s.LineCount - 1)
                    | ValueNone -> ValueNone

                let placement =
                    match surround with
                    | ValueSome s -> SnippetIndentation.AroundSelection(s.Column, s.FieldIndent)
                    | ValueNone -> SnippetIndentation.AtCaret span.iStartIndex

                let lastLine = min span.iEndLine (snapshot.LineCount - 1)

                let lines =
                    [
                        for lineNumber in span.iStartLine .. lastLine ->
                            let line = snapshot.GetLineFromLineNumber lineNumber
                            let indent = leadingWhitespaceOf line

                            let kind =
                                if indent = line.Length then
                                    SnippetIndentation.Blank
                                elif startsRootLevelDirective line indent then
                                    SnippetIndentation.RootLevelDirective
                                else
                                    match selectedLines with
                                    | ValueSome(first, _) when lineNumber = first -> SnippetIndentation.SelectedFirst
                                    | ValueSome(first, last) when lineNumber > first && lineNumber <= last ->
                                        SnippetIndentation.SelectedRest
                                    | _ -> SnippetIndentation.Template

                            {
                                SnippetIndentation.Kind = kind
                                SnippetIndentation.Indent = indent
                            }
                    ]

                use edit = subjectBuffer.CreateEdit()

                SnippetIndentation.deltas placement lines
                |> List.iteri (fun offset delta ->
                    let line = snapshot.GetLineFromLineNumber(span.iStartLine + offset)

                    if delta > 0 then
                        edit.Insert(line.Start.Position, indentTextOf textView.Options delta) |> ignore
                    elif delta < 0 then
                        edit.Delete(line.Start.Position, -delta) |> ignore)

                edit.Apply() |> ignore

            VSConstants.S_OK
