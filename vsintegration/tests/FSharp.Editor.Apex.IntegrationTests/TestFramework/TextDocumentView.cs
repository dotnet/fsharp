//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Test.Apex.VisualStudio.Editor;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Represents the text view of an open VS document. Compact, self-contained equivalent of the
    /// TypeScript-VS TextDocumentView / TextDocumentViewBase, wrapping the Apex text editor extension
    /// with the small set of operations the code-action tests need.
    /// </summary>
    public sealed class TextDocumentView
    {
        private readonly IVisualStudioTextEditorTestExtension editor;

        public TextDocumentView(IVisualStudioTextEditorTestExtension editor)
        {
            this.editor = editor;
        }

        /// <summary>The full text content of the document.</summary>
        public string Contents => this.editor.Contents;

        /// <summary>
        /// Inserts text verbatim at the current caret position via a direct text-buffer edit.
        /// Unlike <c>InsertTextWithReturn</c>, this does not simulate per-character typing, so the F#
        /// editor's brace/quote auto-completion, IntelliSense auto-commit and smart indentation do not
        /// fire and rewrite the buffer. That keeps the inserted source exactly as given, which the
        /// code-fix tests rely on when locating expressions afterwards.
        /// </summary>
        public void InsertText(string text) => this.editor.Edit.InsertTextInBuffer(text);

        /// <summary>Moves the caret to the first (or <paramref name="matchIndex"/>-th) occurrence of an expression.</summary>
        public void MoveToExpression(string expression, int matchIndex = 1)
            => this.editor.Caret.MoveToExpression(expression, matchIndex: matchIndex);

        /// <summary>Moves the caret one character to the left.</summary>
        public void MoveLeft() => this.editor.Caret.MoveLeft();

        /// <summary>Moves the caret one character to the right.</summary>
        public void MoveRight() => this.editor.Caret.MoveRight();
    }
}
