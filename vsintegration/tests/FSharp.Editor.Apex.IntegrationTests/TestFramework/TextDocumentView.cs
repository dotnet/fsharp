//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Shell;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Represents the text view of an open VS document. Compact, self-contained equivalent of the
    /// TypeScript-VS TextDocumentView / TextDocumentViewBase, wrapping the Apex text editor extension
    /// with the small set of operations the code-action tests need.
    /// </summary>
    public sealed class TextDocumentView
    {
        private readonly TextEditorDocumentWindowTestExtension window;

        public TextDocumentView(TextEditorDocumentWindowTestExtension window)
        {
            this.window = window;
        }

        private IVisualStudioTextEditorTestExtension editor => this.window.Editor;

        /// <summary>The full text content of the document.</summary>
        public string Contents => this.editor.Contents;

        /// <summary>The path of the file backing this document.</summary>
        public string FilePath => this.window.FilePath;

        /// <summary>
        /// Inserts text verbatim at the current caret position via a direct text-buffer edit.
        /// Unlike <c>InsertTextWithReturn</c>, this does not simulate per-character typing, so the F#
        /// editor's brace/quote auto-completion, IntelliSense auto-commit and smart indentation do not
        /// fire and rewrite the buffer. That keeps the inserted source exactly as given, which the
        /// code-fix tests rely on when locating expressions afterwards.
        /// </summary>
        public void InsertText(string text) => this.editor.Edit.InsertTextInBuffer(text);

        /// <summary>
        /// Moves the caret to the first (or <paramref name="matchIndex"/>-th) occurrence of an
        /// expression. Apex's caret search treats the argument as a regular expression, but every caller
        /// passes a literal source fragment (which may contain regex metacharacters such as ')' in
        /// "SomeType)"), so the input is escaped to match it literally.
        /// </summary>
        public void MoveToExpression(string expression, int matchIndex = 1)
            => this.editor.Caret.MoveToExpression(Regex.Escape(expression), matchIndex: matchIndex);

        /// <summary>Moves the caret one character to the left.</summary>
        public void MoveLeft() => this.editor.Caret.MoveLeft();

        /// <summary>Moves the caret one character to the right.</summary>
        public void MoveRight() => this.editor.Caret.MoveRight();

        /// <summary>Gives this document's editor keyboard focus (needed before caret-driven commands).</summary>
        public void Focus() => this.editor.Focus();

        /// <summary>The text of the line the caret is currently on (untrimmed).</summary>
        public string CurrentLineText => this.editor.Caret.GetCurrentLineText();

        /// <summary>Invokes Go To Definition from the current caret position.</summary>
        public void GoToDefinition() => this.editor.Caret.GoToDefinition();

        /// <summary>
        /// Replaces the entire document with <paramref name="text"/> and saves it to disk. Build tests
        /// need this: the template auto-opens the default file, so the source must be set through the
        /// open document (buffer) and saved, otherwise an out-of-band disk write is superseded by the
        /// still-open (valid template) buffer and the compiler never sees the intended code.
        ///
        /// The template content loads into the buffer asynchronously after the document opens, so this
        /// first waits for that initial content to arrive and then applies the whole-buffer replacement
        /// with verify-and-retry: without this a replacement done too early is overwritten by the
        /// late-arriving template content, leaving the original source on disk. The selection is deleted
        /// before a direct buffer insert (which does not itself replace a selection and, unlike simulated
        /// typing, avoids brace/quote auto-completion).
        /// </summary>
        public void ReplaceAllAndSave(string text)
        {
            this.editor.Focus();

            if (!WaitUntil(() => !string.IsNullOrEmpty(this.editor.Contents)))
            {
                throw new InvalidOperationException("Document did not load its initial content before editing.");
            }

            var replaced = WaitUntil(() =>
            {
                this.editor.Selection.SelectAll();
                this.editor.Edit.DeleteCharNext(1);
                this.editor.Edit.InsertTextInBuffer(text);
                return Normalize(this.editor.Contents) == Normalize(text);
            });

            if (!replaced)
            {
                throw new InvalidOperationException(
                    $"Failed to set document content. Expected:\n{text}\nActual:\n{this.editor.Contents}");
            }

            this.window.Save();

            if (!this.window.TryWaitForNotDirty(TimeSpan.FromSeconds(15), TimeSpan.FromMilliseconds(100)))
            {
                throw new InvalidOperationException("Document remained dirty after saving.");
            }
        }

        private static bool WaitUntil(Func<bool> condition)
        {
            var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            do
            {
                if (condition())
                {
                    return true;
                }

                Thread.Sleep(200);
            }
            while (DateTime.UtcNow < deadline);

            return false;
        }

        private static string Normalize(string source)
            => (source ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');
    }
}
