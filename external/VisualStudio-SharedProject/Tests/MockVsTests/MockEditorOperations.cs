/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using TestUtilities.Mocks;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockEditorOperations : IEditorOperations {
        private readonly MockTextView _view;
        
        public MockEditorOperations(MockTextView textView) {
            _view = textView;
        }

        public void AddAfterTextBufferChangePrimitive() {
            throw new NotImplementedException();
        }

        public void AddBeforeTextBufferChangePrimitive() {
            throw new NotImplementedException();
        }

        public bool Backspace() {
            throw new NotImplementedException();
        }

        public bool CanCut {
            get { throw new NotImplementedException(); }
        }

        public bool CanDelete {
            get { throw new NotImplementedException(); }
        }

        public bool CanPaste {
            get { throw new NotImplementedException(); }
        }

        public bool Capitalize() {
            throw new NotImplementedException();
        }

        public bool ConvertSpacesToTabs() {
            throw new NotImplementedException();
        }

        public bool ConvertTabsToSpaces() {
            throw new NotImplementedException();
        }

        public bool CopySelection() {
            throw new NotImplementedException();
        }

        public bool CutFullLine() {
            throw new NotImplementedException();
        }

        public bool CutSelection() {
            throw new NotImplementedException();
        }

        public bool DecreaseLineIndent() {
            throw new NotImplementedException();
        }

        public bool Delete() {
            throw new NotImplementedException();
        }

        public bool DeleteBlankLines() {
            throw new NotImplementedException();
        }

        public bool DeleteFullLine() {
            throw new NotImplementedException();
        }

        public bool DeleteHorizontalWhiteSpace() {
            throw new NotImplementedException();
        }

        public bool DeleteToBeginningOfLine() {
            throw new NotImplementedException();
        }

        public bool DeleteToEndOfLine() {
            throw new NotImplementedException();
        }

        public bool DeleteWordToLeft() {
            throw new NotImplementedException();
        }

        public bool DeleteWordToRight() {
            throw new NotImplementedException();
        }

        public void ExtendSelection(int newEnd) {
            throw new NotImplementedException();
        }

        public string GetWhitespaceForVirtualSpace(VisualStudio.Text.VirtualSnapshotPoint point) {
            throw new NotImplementedException();
        }

        public void GotoLine(int lineNumber) {
            throw new NotImplementedException();
        }

        public bool IncreaseLineIndent() {
            throw new NotImplementedException();
        }

        public bool Indent() {
            throw new NotImplementedException();
        }

        public bool InsertFile(string filePath) {
            throw new NotImplementedException();
        }

        public bool InsertNewLine() {
            return InsertText(_view.Options.GetNewLineCharacter());
        }

        public bool InsertProvisionalText(string text) {
            throw new NotImplementedException();
        }

        public bool InsertText(string text) {
            _view.TextBuffer.Insert(_view.Caret.Position.BufferPosition.Position, text);
            return true;
        }

        public bool InsertTextAsBox(string text, out VisualStudio.Text.VirtualSnapshotPoint boxStart, out VisualStudio.Text.VirtualSnapshotPoint boxEnd) {
            throw new NotImplementedException();
        }

        public bool MakeLowercase() {
            throw new NotImplementedException();
        }

        public bool MakeUppercase() {
            throw new NotImplementedException();
        }

        public void MoveCaret(VisualStudio.Text.Formatting.ITextViewLine textLine, double horizontalOffset, bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveCurrentLineToBottom() {
            throw new NotImplementedException();
        }

        public void MoveCurrentLineToTop() {
            throw new NotImplementedException();
        }

        public void MoveLineDown(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveLineUp(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToBottomOfView(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToEndOfDocument(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToEndOfLine(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToHome(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToLastNonWhiteSpaceCharacter(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToNextCharacter(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToNextWord(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToPreviousCharacter(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToPreviousWord(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToStartOfDocument(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToStartOfLine(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToStartOfLineAfterWhiteSpace(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToStartOfNextLineAfterWhiteSpace(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToStartOfPreviousLineAfterWhiteSpace(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void MoveToTopOfView(bool extendSelection) {
            throw new NotImplementedException();
        }

        public bool NormalizeLineEndings(string replacement) {
            throw new NotImplementedException();
        }

        public bool OpenLineAbove() {
            throw new NotImplementedException();
        }

        public bool OpenLineBelow() {
            throw new NotImplementedException();
        }

        public VisualStudio.Text.Editor.IEditorOptions Options {
            get { throw new NotImplementedException(); }
        }

        public void PageDown(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void PageUp(bool extendSelection) {
            throw new NotImplementedException();
        }

        public bool Paste() {
            throw new NotImplementedException();
        }

        public VisualStudio.Text.ITrackingSpan ProvisionalCompositionSpan {
            get { throw new NotImplementedException(); }
        }

        public int ReplaceAllMatches(string searchText, string replaceText, bool matchCase, bool matchWholeWord, bool useRegularExpressions) {
            throw new NotImplementedException();
        }

        public bool ReplaceSelection(string text) {
            throw new NotImplementedException();
        }

        public bool ReplaceText(VisualStudio.Text.Span replaceSpan, string text) {
            throw new NotImplementedException();
        }

        public void ResetSelection() {
            throw new NotImplementedException();
        }

        public void ScrollColumnLeft() {
            throw new NotImplementedException();
        }

        public void ScrollColumnRight() {
            throw new NotImplementedException();
        }

        public void ScrollDownAndMoveCaretIfNecessary() {
            throw new NotImplementedException();
        }

        public void ScrollLineBottom() {
            throw new NotImplementedException();
        }

        public void ScrollLineCenter() {
            throw new NotImplementedException();
        }

        public void ScrollLineTop() {
            throw new NotImplementedException();
        }

        public void ScrollPageDown() {
            throw new NotImplementedException();
        }

        public void ScrollPageUp() {
            throw new NotImplementedException();
        }

        public void ScrollUpAndMoveCaretIfNecessary() {
            throw new NotImplementedException();
        }

        public void SelectAll() {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VisualStudio.Text.VirtualSnapshotPoint anchorPoint, VisualStudio.Text.VirtualSnapshotPoint activePoint, VisualStudio.Text.Editor.TextSelectionMode selectionMode, VisualStudio.Text.Editor.EnsureSpanVisibleOptions? scrollOptions) {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VisualStudio.Text.VirtualSnapshotPoint anchorPoint, VisualStudio.Text.VirtualSnapshotPoint activePoint, VisualStudio.Text.Editor.TextSelectionMode selectionMode) {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VisualStudio.Text.VirtualSnapshotPoint anchorPoint, VisualStudio.Text.VirtualSnapshotPoint activePoint) {
            throw new NotImplementedException();
        }

        public void SelectCurrentWord() {
            throw new NotImplementedException();
        }

        public void SelectEnclosing() {
            throw new NotImplementedException();
        }

        public void SelectFirstChild() {
            throw new NotImplementedException();
        }

        public void SelectLine(VisualStudio.Text.Formatting.ITextViewLine viewLine, bool extendSelection) {
            throw new NotImplementedException();
        }

        public void SelectNextSibling(bool extendSelection) {
            throw new NotImplementedException();
        }

        public void SelectPreviousSibling(bool extendSelection) {
            throw new NotImplementedException();
        }

        public string SelectedText {
            get { throw new NotImplementedException(); }
        }

        public void SwapCaretAndAnchor() {
            throw new NotImplementedException();
        }

        public bool Tabify() {
            throw new NotImplementedException();
        }

        public VisualStudio.Text.Editor.ITextView TextView {
            get { throw new NotImplementedException(); }
        }

        public bool ToggleCase() {
            throw new NotImplementedException();
        }

        public bool TransposeCharacter() {
            throw new NotImplementedException();
        }

        public bool TransposeLine() {
            throw new NotImplementedException();
        }

        public bool TransposeWord() {
            throw new NotImplementedException();
        }

        public bool Unindent() {
            throw new NotImplementedException();
        }

        public bool Untabify() {
            throw new NotImplementedException();
        }

        public void ZoomIn() {
            throw new NotImplementedException();
        }

        public void ZoomOut() {
            throw new NotImplementedException();
        }

        public void ZoomTo(double zoomLevel) {
            throw new NotImplementedException();
        }
    }
}
