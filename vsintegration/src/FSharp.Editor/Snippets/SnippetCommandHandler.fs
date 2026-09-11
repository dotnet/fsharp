// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.Commanding
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Editor.Commanding.Commands
open Microsoft.VisualStudio.Utilities

open FSharp.Compiler.EditorServices

open CancellableTasks

[<AutoOpen>]
module internal SnippetCommandHelpers =

    [<Literal>]
    let private userOpName = "FSharpSnippetCommandHandler"

    /// The snippet shortcut the caret is sitting at the end of. Going through the lexer is what keeps
    /// Tab from expanding a word typed inside a string or a comment, and `FullIsland` is what keeps it
    /// from expanding the member name in `value.for`.
    let tryGetShortcutAt (document: Document) position =
        cancellableTask {
            let! symbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)

            return
                match symbol with
                | Some symbol when symbol.FullIsland.Length = 1 -> ValueSome(symbol.Ident.idText, symbol.Ident.idRange)
                | _ -> ValueNone
        }

    /// Drops a trailing line break from the selection, changing what is selected and nothing else.
    ///
    /// Selecting whole lines ends the selection at column 0 of the following one, so `$selected$`
    /// receives that line break too: whatever the snippet places after the field - `#endif`, `else`,
    /// a closing `}` - lands on the line that followed the selection instead of on its own.
    let trimSelectedLineBreak (textView: ITextView) =
        let selection = textView.Selection
        let span = selection.StreamSelectionSpan.SnapshotSpan
        let endLine = span.Snapshot.GetLineFromPosition span.End.Position

        if span.End.Position = endLine.Start.Position && span.Length > 0 then
            let trimmed =
                SnapshotSpan(span.Start, span.Snapshot.GetLineFromLineNumber(endLine.LineNumber - 1).End)

            // Caret first: moving it collapses the selection.
            textView.Caret.MoveTo trimmed.End |> ignore
            textView.Selection.Select(trimmed, selection.IsReversed)

    /// The column the wrapped code sits at and how many lines it covers. The insertion replaces the
    /// selection, so neither survives it and the expansion client is told up front.
    ///
    /// The column is the narrowest indentation in the block, not the first line's: the wrapper belongs
    /// at the block's own left edge even when the block starts with a deeper line.
    let selectionShape (textView: ITextView) =
        let span = textView.Selection.StreamSelectionSpan.SnapshotSpan
        let snapshot = span.Snapshot
        let firstLine = snapshot.GetLineFromPosition span.Start.Position
        let lastLine = snapshot.GetLineFromPosition span.End.Position

        let column =
            seq { firstLine.LineNumber .. lastLine.LineNumber }
            |> Seq.fold
                (fun narrowest lineNumber ->
                    let line = snapshot.GetLineFromLineNumber lineNumber

                    match leadingWhitespaceOf line with
                    | indent when indent = line.Length -> narrowest
                    | indent -> min narrowest indent)
                Int32.MaxValue

        let column = if column = Int32.MaxValue then 0 else column

        column, lastLine.LineNumber - firstLine.LineNumber + 1

/// Insert Snippet (Ctrl+K,Ctrl+X), Surround With (Ctrl+K,Ctrl+S), Tab expansion of a snippet
/// shortcut, and the keys that drive a live expansion session.
///
/// Ordered after the completion handler so that Tab still commits an open completion list first,
/// which is how the C# handler is ordered too.
[<Export(typeof<ICommandHandler>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<Name(Constants.FSharpSnippetsCommandHandler)>]
[<Order(After = PredefinedCompletionNames.CompletionCommandHandler)>]
type internal FSharpSnippetCommandHandler [<ImportingConstructor>] (editorAdapters: IVsEditorAdaptersFactoryService) =

    let tryGetClient (textView: ITextView) (subjectBuffer: ITextBuffer) =
        match textView with
        | :? IWpfTextView as wpfTextView ->
            ValueSome(
                wpfTextView.Properties.GetOrCreateSingletonProperty<FSharpSnippetExpansionClient>(fun () ->
                    FSharpSnippetExpansionClient(wpfTextView, subjectBuffer, editorAdapters))
            )
        | _ -> ValueNone

    /// Only a live session gets to see Tab, Shift+Tab, Enter and Escape.
    let tryGetSessionClient (textView: ITextView) (subjectBuffer: ITextBuffer) =
        tryGetClient textView subjectBuffer |> ValueOption.filter _.IsInSession

    let tryExpandShortcut (args: TabKeyCommandArgs) (client: FSharpSnippetExpansionClient) =
        match args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges() with
        | null -> false
        | document when not document.Project.IsFSharp -> false
        | document ->
            let caret = args.TextView.Caret.Position.BufferPosition.Position

            match runSynchronously parseTimeout (tryGetShortcutAt document caret) with
            | ValueNone -> false
            | ValueSome(shortcut, range) ->
                let shortcutSpan =
                    VsTextSpan(
                        iStartLine = range.StartLine - 1,
                        iStartIndex = range.StartColumn,
                        iEndLine = range.EndLine - 1,
                        iEndIndex = range.EndColumn
                    )

                client.TryInsertExpansionForShortcut(shortcut, shortcutSpan)

    // `ICommandHandler<_>` inherits `INamed`, so the name is given once for all six of them.
    interface INamed with
        member _.DisplayName = Constants.FSharpSnippetsCommandHandler

    interface ICommandHandler<InsertSnippetCommandArgs> with
        member _.GetCommandState _ = CommandState.Available

        member _.ExecuteCommand(args, _) =
            tryGetClient args.TextView args.SubjectBuffer
            |> ValueOption.exists (fun client -> client.TryInsertSnippet())

    interface ICommandHandler<SurroundWithCommandArgs> with
        member _.GetCommandState args =
            if args.TextView.Selection.IsEmpty then
                CommandState.Unavailable
            else
                CommandState.Available

        // The buffer is left alone - the expansion engine reads the selection off the view to fill
        // `$selected$`, and editing first was what yanked the code to column 0.
        member _.ExecuteCommand(args, _) =
            trimSelectedLineBreak args.TextView
            let column, lineCount = selectionShape args.TextView

            tryGetClient args.TextView args.SubjectBuffer
            |> ValueOption.exists (fun client -> client.TrySurroundWith(column, lineCount))

    interface ICommandHandler<TabKeyCommandArgs> with
        member _.GetCommandState _ = CommandState.Unspecified

        member _.ExecuteCommand(args, _) =
            match tryGetSessionClient args.TextView args.SubjectBuffer with
            | ValueSome client -> client.TryHandleTab()
            | ValueNone ->
                args.TextView.Selection.IsEmpty
                && (tryGetClient args.TextView args.SubjectBuffer
                    |> ValueOption.exists (tryExpandShortcut args))

    interface ICommandHandler<BackTabKeyCommandArgs> with
        member _.GetCommandState _ = CommandState.Unspecified

        member _.ExecuteCommand(args, _) =
            tryGetSessionClient args.TextView args.SubjectBuffer
            |> ValueOption.exists (fun client -> client.TryHandleBackTab())

    interface ICommandHandler<ReturnKeyCommandArgs> with
        member _.GetCommandState _ = CommandState.Unspecified

        member _.ExecuteCommand(args, _) =
            tryGetSessionClient args.TextView args.SubjectBuffer
            |> ValueOption.exists (fun client -> client.TryHandleReturn())

    interface ICommandHandler<EscapeKeyCommandArgs> with
        member _.GetCommandState _ = CommandState.Unspecified

        member _.ExecuteCommand(args, _) =
            tryGetSessionClient args.TextView args.SubjectBuffer
            |> ValueOption.exists (fun client -> client.TryHandleEscape())
