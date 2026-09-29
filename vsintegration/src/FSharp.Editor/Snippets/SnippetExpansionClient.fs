// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Xml.Linq

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor.DebugHelpers
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods
open Microsoft.VisualStudio.TextManager.Interop

open MSXML

[<AutoOpen>]
module internal SnippetExpansionHelpers =

    let leadingWhitespaceOf (line: ITextSnapshotLine) =
        let snapshot = line.Snapshot
        let start = line.Start.Position
        let mutable width = 0

        while width < line.Length && Char.IsWhiteSpace snapshot[start + width] do
            width <- width + 1

        width

    let indentTextOf (options: IEditorOptions) width =
        if options.IsConvertTabsToSpacesEnabled() then
            String(' ', width)
        else
            let tabSize = options.GetTabSize()
            String('\t', width / tabSize) + String(' ', width % tabSize)

    /// Where `$selected$` sits in a snippet's `<Code>`: which of its lines holds the field, and the
    /// column the template indents it to. The expansion session will not report it, so it is read
    /// from the file the picker named.
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
        FieldLine: int
        FieldIndent: int
    }

type internal FSharpSnippetExpansionClient
    (textView: IWpfTextView, subjectBuffer: ITextBuffer, editorAdapters: IVsEditorAdaptersFactoryService) =

    let languageGuid = Guid FSharpConstants.languageServiceGuidString

    /// Set from `OnBeforeInsertion` rather than from `InsertNamedExpansion`'s out parameter: a snippet
    /// with no editable fields ends its session from inside that call, so the out parameter arrives
    /// after `EndExpansion` has already run.
    let mutable expansionSession: IVsExpansionSession = null

    /// What the open picker was invoked over, before the template is known: `(column, lineCount)`
    /// for Surround With, ValueNone for Insert Snippet.
    let mutable pendingSurround: (int * int) voption = ValueNone

    /// The same, completed with the chosen template's layout once the picker has answered. ValueNone
    /// for Insert Snippet, where the caret column is the whole answer.
    let mutable surround: SurroundLayout voption = ValueNone

    /// `FormatSpan` can be called more than once per session, and it inserts, so it must run once.
    let mutable indentPending = false

    member _.IsInSession =
        match expansionSession with
        | null -> false
        | _ -> true

    /// `tsInsertPos` is the range `InsertNamedExpansion` *replaces*, so handing it the selection
    /// deletes the text a SurroundsWith snippet was meant to wrap. The engine reads the selection off
    /// the view it was given in `InvokeInsertionUI` to fill `$selected$`.
    member private _.TryGetCaretSpan() =
        if not (obj.ReferenceEquals(textView.TextBuffer, subjectBuffer)) then
            // Nothing projects F# today; bail out rather than guess at a mapping.
            ValueNone
        else
            let caret = textView.Caret.Position.BufferPosition
            let line = caret.Snapshot.GetLineFromPosition caret.Position
            let column = caret.Position - line.Start.Position

            ValueSome(VsTextSpan(iStartLine = line.LineNumber, iStartIndex = column, iEndLine = line.LineNumber, iEndIndex = column))

    member private this.InsertNamedExpansion(title, path, insertionSpan: VsTextSpan, selection) =
        match editorAdapters.GetBufferAdapter subjectBuffer with
        | :? IVsExpansion as expansion ->
            surround <-
                match selection, tryReadSelectedFieldLayout path with
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

            let hr, _session =
                expansion.InsertNamedExpansion(title, path, insertionSpan, this, languageGuid, 0)

            ErrorHandler.Succeeded hr
        | _ -> false

    member this.TryInsertExpansionForShortcut(shortcut: string, shortcutSpan: VsTextSpan) =
        match ServiceProvider.GlobalProvider.ExpansionManager, editorAdapters.GetViewAdapter textView with
        | null, _
        | _, null -> false
        | expansionManager, viewAdapter ->
            let spans = [| shortcutSpan |]

            match expansionManager.GetExpansionByShortcut(this, languageGuid, shortcut, viewAdapter, spans, 0) with
            | _, null, _ -> false
            | hr, path, title ->
                ErrorHandler.Succeeded hr
                && this.InsertNamedExpansion(title, path, spans[0], ValueNone)

    /// It is not modal: the chosen item comes back later through `OnItemChosen`.
    member private this.InvokeInsertionUI(types: string[], prompt, selection) =
        match ServiceProvider.GlobalProvider.ExpansionManager, editorAdapters.GetViewAdapter textView with
        | null, _
        | _, null -> false
        | expansionManager, viewAdapter ->
            pendingSurround <- selection

            let hr =
                expansionManager.InvokeInsertionUI(viewAdapter, this, languageGuid, types, types.Length, 1, null, 0, 0, prompt, null)

            not (ErrorHandler.Failed hr)

    member this.TryInsertSnippet() =
        this.InvokeInsertionUI([| "Expansion"; "SurroundsWith" |], SR.InsertSnippet(), ValueNone)

    member this.TrySurroundWith(column: int, lineCount: int) =
        this.InvokeInsertionUI([| "SurroundsWith" |], SR.SurroundWith(), ValueSome(column, lineCount))

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

    member private _.Reindent(span: VsTextSpan) =
        let snapshot = subjectBuffer.CurrentSnapshot
        let tabSize = textView.Options.GetTabSize()

        let lines =
            [
                for lineNumber in span.iStartLine .. min span.iEndLine (snapshot.LineCount - 1) -> snapshot.GetLineFromLineNumber lineNumber
            ]

        let texts = lines |> List.map _.GetText()

        // `GetFieldSpan "selected"` does not answer for that special literal.
        let placement, selectedLines =
            match surround with
            | ValueSome layout ->
                SnippetIndentation.AroundSelection(layout.Column, layout.FieldIndent),
                ValueSome(layout.FieldLine, layout.FieldLine + layout.LineCount - 1)
            | ValueNone ->
                let caretColumn =
                    texts.Head
                    |> Seq.take span.iStartIndex
                    |> Seq.fold (SnippetIndentation.advanceColumn tabSize) 0

                SnippetIndentation.AtCaret caretColumn, ValueNone

        use edit = subjectBuffer.CreateEdit()

        SnippetIndentation.classify tabSize selectedLines texts
        |> SnippetIndentation.deltas placement
        |> List.iter2
            (fun (line: ITextSnapshotLine) delta ->
                match delta with
                | 0 -> ()
                | indent when indent > 0 -> edit.Insert(line.Start.Position, indentTextOf textView.Options indent) |> ignore
                // A negative delta unindents the line entirely, and the indent is measured
                // in columns while the edit removes characters.
                | _ -> edit.Delete(line.Start.Position, leadingWhitespaceOf line) |> ignore)
            lines

        edit.Apply() |> ignore

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
            VSConstants.S_OK

        member this.OnItemChosen(pszTitle, pszPath) =
            match this.TryGetCaretSpan() with
            | ValueSome span -> this.InsertNamedExpansion(pszTitle, pszPath, span, pendingSurround) |> ignore
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

        member this.FormatSpan(_buffer, ts: VsTextSpan[]) =
            if indentPending && ts.Length > 0 then
                indentPending <- false
                this.Reindent(ts[0])

            VSConstants.S_OK
