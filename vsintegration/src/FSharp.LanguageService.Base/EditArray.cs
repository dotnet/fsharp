// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    /// <summary>
    /// This class encapsulates one atomic edit operation.
    /// Add these to an EditArray then when you are ready call ApplyEdits().
    /// </summary>
    internal class EditSpan {
        TextSpan span; // existing span to delete
        string text; // new text to insert.
        int lineCount;
        int lengthOfLastLine;

        /// <summary>
        /// Construct a new edit span object
        /// </summary>
        /// <param name="toReplace">The text span to remove from the buffer (can be empty)</param>
        /// <param name="insertText">The text to insert in it's place (can be null)</param>
        internal EditSpan(TextSpan toReplace, string insertText) {
            if (!TextSpanHelper.IsPositive(toReplace)) {
                TextSpanHelper.MakePositive(ref toReplace);
            }
            this.span = toReplace;
            this.text = insertText;
            this.lineCount = -1;
        }

        internal TextSpan Span {
            get { return this.span; }
            set { this.span = value; }
        }
        internal string Text {
            get { return this.text; }
            set { this.text = value; this.lineCount = -1; }
        }

        /// <summary>
        /// Returns the number of lines in the new text being inserted.
        /// </summary>
        internal int LineCount {
            get {
                // number of newlines in the inserted text.
                if (this.lineCount == -1) CalcLines();
                return this.lineCount;
            }
        }

        /// <summary>
        /// Returns the length of the last line of text being inserted.
        /// </summary>
        internal int LengthOfLastLine { // length of the last line of text.
            get {
                // number of newlines in the inserted text.
                if (this.lineCount == -1) CalcLines();
                return this.lengthOfLastLine;
            }
        }

        void CalcLines() {
            int pos = 0;
            this.lineCount = 0;
            for (int j = 0, m = text.Length; j < m; j++) {
                char ch = text[j];
                if (ch == '\r' || ch == '\n') {
                    if (ch == '\r' && j + 1 < m && text[j + 1] == '\n') {
                        j++; // treat '\r\n' as a single line.
                    }
                    this.lineCount++;
                    pos = 0;
                } else {
                    pos++;
                }
            }
            this.lengthOfLastLine = pos;
        }

    }

    /// <summary>
    /// This class encapsulates a batch edit operation.  The reason this class exists is because
    /// performing thousands of tiny edits on a large document can be pretty slow, so the best thing
    /// to do is merge the edits into bigger chunks and that is exactly what this class will do
    /// for you.  The trick is that when merging edits you need to be careful not to include any 
    /// IVsTextLineMarkers in the merged chunks, because editing over the top of the marker
    /// will blow it away, which is not what the user wants.  The user wants to keep all their
    /// breakpoints and bookmarks, and red and blue squigglies and so on.  So this class also takes
    /// care of that.
    /// </summary>
    internal class EditArray : IEnumerable, IDisposable {
        ArrayList editList;
        ISource source;
        TextSpan selection;
        bool merge;
        IVsTextView view;
        string description;
        int changeCount;
        CompoundAction ca;
        CompoundViewAction cva;

        /// <summary>
        /// This constructor takes a view and will use CompoundViewAction to make the updates
        /// and it will update the current selection accordingly.
        /// <param name="source">The buffer to operate on</param>
        /// <param name="view">The text view to use for CompoundViewAction and whose selection you want updated</param>
        /// <param name="merge">Whether to attempt to merge edits</param>
        /// <param name="description">Name used in compound action</param>
        /// </summary>
        internal EditArray(ISource source, IVsTextView view, bool merge, string description) {
            
            this.source = source;
            this.editList = new ArrayList();
            this.merge = merge;
            this.description = description;

            if (view != null) {
                TextSpan[] pSpan = new TextSpan[1];
                view.GetSelectionSpan(pSpan);
                this.selection = pSpan[0];
                this.view = view;

                this.cva = new CompoundViewAction(this.view, description);
                this.cva.FlushEditActions(); // make sure we see a consistent coordinate system.
            } else {
                this.ca = new CompoundAction(this.source, description);
                this.ca.FlushEditActions();
            }

            // Sanity check - make sure others are not modifying the buffer while the
            // caller is preparing the big edit operation.
            this.changeCount = source.ChangeCount;
        }

        ~EditArray() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (cva != null) {
                cva.Close();
                cva = null;
            }
            if (ca != null) {
                ca.Close();
                ca = null;
            }
            view = null;
            source = null;
        }

        public void Dispose() {
            Dispose(true);
        }

        /// <summary>
        /// Return the number of edits in the array.
        /// </summary>
        internal int Count {
            get {
                return this.editList.Count;
            }
        }

        internal IVsTextView TextView {
            get {
                return this.view;
            }
        }

        internal ISource Source {
            get {
                return this.source;
            }
        }

        public override string ToString() {
            StringBuilder s=new StringBuilder();
            for (int i = this.editList.Count - 1; i >= 0; i--) {
                EditSpan e = (EditSpan)this.editList[i];
                s.AppendFormat("({0},{1}:{2},{3})  >>> '{4}'",e.Span.iStartLine, e.Span.iStartIndex, e.Span.iEndLine, e.Span.iEndIndex, GetDebugString(e.Text));
                s.AppendLine();
            }
            return s.ToString();
        }

        /// <summary>
        /// Add a new atomic edit to the array.  The edits cannot intersect each other.  
        /// The spans in each edit must be based on the current state of the buffer, 
        /// and not based on post-edit spans.  This EditArray will calculate the
        /// post edit spans for you.
        /// </summary>
        /// <param name="editSpan"></param>
        internal void Add(EditSpan editSpan) {
            if (editSpan == null) {
                throw new ArgumentNullException("editSpan");
            }

            for (int i = this.editList.Count - 1; i>=0; i--){
                EditSpan e = (EditSpan)this.editList[i];
                if (TextSpanHelper.Intersects(editSpan.Span, e.Span)){
                    string msg = SR.GetString(SR.EditIntersects, i);
#if LANGTRACE
                    Debug.Assert(false, msg);
                    TraceEdits();
#endif
                    throw new System.ArgumentException(msg);
                }
                if (TextSpanHelper.StartsAfterStartOf(editSpan.Span, e.Span)) {
                    this.editList.Insert(i + 1, editSpan);
                    return;
                }
            }
            this.editList.Insert(0, editSpan);
        }
#if LANGTRACE
        void TraceEdits() {
            for (int j = 0; j < this.editList.Count - 1; j++) {
                EditSpan f = (EditSpan)this.editList[j];
                TextSpan span = f.Span;
                string t = this.source.GetText(span);
                Trace.WriteLine(
                    string.Format("{0}: {1},{2},{3},{4} '{5}'=>'{6}'",
                        j, span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex,
                        GetDebugString(t), GetDebugString(f.Text)));
            }
        }
#endif
        string GetDebugString(string s) {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Replace("\n", "\\n");
            s = s.Replace("\r", "\\r");
            s = s.Replace("\t", "\\t");
            s = s.Replace(" ", "#");
            return s;
        }

        ArrayList GetTextMarkers() {
            ArrayList markers = new ArrayList();
            TextSpan docSpan = this.source.GetDocumentSpan();
            IVsTextLines buffer = this.source.GetTextLines();
            IVsEnumLineMarkers ppEnum;
            int iMarkerType = 0;
            uint dwFlags = (uint)ENUMMARKERFLAGS.EM_ALLTYPES;
            int hr = buffer.EnumMarkers(docSpan.iStartLine, docSpan.iStartIndex, docSpan.iEndLine, docSpan.iEndIndex, iMarkerType, dwFlags, out ppEnum);
            if (hr == NativeMethods.S_OK) {
                IVsTextLineMarker marker;
                TextSpan[] pSpan = new TextSpan[1];
                while (ppEnum.Next(out marker) == NativeMethods.S_OK) {
                    if (marker != null) {
                        if (marker.GetCurrentSpan(pSpan) == NativeMethods.S_OK) {
                            markers.Add(pSpan[0]);
                        }
                    }
                }
            }
            return markers;
        }

        const int ChunkThreshold = 1000; // don't combine chunks separate by more than 1000 characters.

        ArrayList MergeEdits(ArrayList edits) {

            StringBuilder buffer = new StringBuilder();
            EditSpan combined = null;
            ArrayList merged = new ArrayList();
            ArrayList markers = GetTextMarkers();
            int markerPos = 0;
            TextSpan marker = (markers.Count > 0) ? (TextSpan)markers[0] : new TextSpan();

            foreach (EditSpan editSpan in edits) {

                TextSpan span = editSpan.Span;
                string text = editSpan.Text;

                if (markerPos < markers.Count &&
                    (TextSpanHelper.StartsAfterStartOf(span, marker) || TextSpanHelper.EndsAfterStartOf(span, marker))) {
                    AddCombinedEdit(combined, buffer, merged);
                    if (TextSpanHelper.Intersects(span, marker)) {
                        combined = null;
                        // Have to apply this as a distinct edit operation.
                        merged.Add(editSpan);
                    } else {
                        combined = editSpan;
                        buffer.Append(text);
                    }
                    while (++markerPos < markers.Count) {
                        marker = (TextSpan)markers[markerPos];
                        if (!TextSpanHelper.StartsAfterStartOf(span, marker) && !TextSpanHelper.EndsAfterStartOf(span, marker)) {
                            break;
                        }
                    }
                } else if (combined == null) {
                    combined = editSpan;
                    buffer.Append(text);
                } else {
                    // A little sanity check here, if there are too many characters in between the two 
                    // edits, then keep them separate.
                    int startOffset = source.GetPositionOfLineIndex(combined.Span.iEndLine, combined.Span.iEndIndex);
                    int endOffset = source.GetPositionOfLineIndex(span.iStartLine, span.iStartIndex);
                    if (endOffset - startOffset > ChunkThreshold) {
                        AddCombinedEdit(combined, buffer, merged);
                        combined = editSpan;
                        buffer.Append(text);
                    } else {
                        // merge edit spans by adding the text in-between the current and previous spans.
                        TextSpan s = combined.Span;
                        string between = this.source.GetText(s.iEndLine, s.iEndIndex, span.iStartLine, span.iStartIndex);
                        buffer.Append(between);
                        buffer.Append(text); // and add the new text.
                        s.iEndIndex = span.iEndIndex;
                        s.iEndLine = span.iEndLine;
                        combined.Span = s;
                    }
                }
            }
            AddCombinedEdit(combined, buffer, merged);
            return merged;
        }

        void AddCombinedEdit(EditSpan combined, StringBuilder buffer, ArrayList merged) {
            if (combined != null) {
                // add combined edit span.
                combined.Text = buffer.ToString();
                merged.Add(combined);
                buffer.Length = 0;
            }
        }

        void UpdateSelection(ArrayList edits) {
            int lineDelta = 0;
            int indexDelta = 0;
            int currentLine = 0;
            bool updateStart = true;
            bool updateEnd = true;
            bool selectionIsEmpty = TextSpanHelper.IsEmpty(this.selection);

            foreach (EditSpan es in edits) {
                TextSpan span = es.Span;
                string text = es.Text;
                int lastLine = currentLine;
                int lastDelta = indexDelta;

                if (currentLine != span.iStartLine) {
                    // We have moved to a new line, so the indexDelta is no longer relevant.
                    currentLine = span.iStartLine;
                    indexDelta = 0;
                }

                // Now adjust the span based on the current deltas.
                span.iStartIndex += indexDelta;
                if (currentLine == span.iEndLine) {
                    span.iEndIndex += indexDelta;
                }
                span.iStartLine += lineDelta;
                span.iEndLine += lineDelta;

                if (updateStart) {
                    TextSpan original = es.Span;
                    if (TextSpanHelper.ContainsInclusive(original, this.selection.iStartLine, this.selection.iStartIndex)) {
                        bool atEnd = (this.selection.iStartLine == original.iEndLine &&
                                this.selection.iStartIndex == original.iEndIndex);
                        this.selection.iStartLine = span.iStartLine;
                        this.selection.iStartIndex = span.iStartIndex;
                        if (atEnd){
                            // Selection was positioned at the end of the span, so
                            // skip past the inserted text to approximate that location.
                            if (es.LineCount > 0) {
                                this.selection.iStartLine += es.LineCount;
                                this.selection.iStartIndex = es.LengthOfLastLine;
                            } else {
                                this.selection.iStartIndex += es.LengthOfLastLine;
                            }
                        }
                        updateStart = false; // done
                    } else if (TextSpanHelper.StartsAfterStartOf(original, this.selection)) {
                        if (this.selection.iStartLine == lastLine) {
                            this.selection.iStartIndex += lastDelta;
                        }
                        this.selection.iStartLine += lineDelta;
                        updateStart = false; // done.
                    }
                    if (!updateStart && selectionIsEmpty) {
                        this.selection.iEndLine = this.selection.iStartLine;
                        this.selection.iEndIndex = this.selection.iStartIndex;
                        updateEnd = false; // done
                    }
                }
                if (updateEnd) {
                    TextSpan original = es.Span;
                    if (TextSpanHelper.StartsAfterEndOf(original, this.selection)) {
                        if (this.selection.iEndLine == lastLine) {
                            this.selection.iEndIndex += lastDelta;
                        }
                        this.selection.iEndLine += lineDelta;
                        updateEnd = false; // done.
                    } else if (TextSpanHelper.ContainsInclusive(original, this.selection.iEndLine, this.selection.iEndIndex)) {
                        this.selection.iEndLine = span.iStartLine;
                        this.selection.iEndIndex = span.iStartIndex;
                        // Now include the text we are inserting in the selection
                        if (es.LineCount > 0) {
                            this.selection.iEndLine += es.LineCount;
                            this.selection.iEndIndex = es.LengthOfLastLine;
                        } else {
                            this.selection.iEndIndex += es.LengthOfLastLine;
                        }
                        updateEnd = false; // done.
                    }
                }

                // Now adjust the deltas based on whether we just deleted anything.
                if (span.iStartLine != span.iEndLine) {
                    // We are deleting one or more lines.
                    lineDelta += (span.iStartLine - span.iEndLine);
                    indexDelta = -span.iEndIndex;
                    currentLine = span.iStartLine;
                } else if (span.iStartIndex != span.iEndIndex) {
                    indexDelta += (span.iStartIndex - span.iEndIndex);
                }

                // Now adjust the deltas based on what we just inserted
                if (!string.IsNullOrEmpty(text)) {
                    lineDelta += es.LineCount;
                    if (span.iStartLine != span.iEndLine) { // we removed multiple lines
                        if (es.LineCount == 0) { // but we are not inserting any new lines
                            // Then we are appending to this line.
                            indexDelta = span.iStartIndex + es.LengthOfLastLine;
                        } else {
                            indexDelta = es.LengthOfLastLine; // otherwise we just started a new line.
                        }
                    } else if (es.LineCount != 0) { // we inserted new lines
                        // then calculate delta between new position versus position on original line.
                        indexDelta += es.LengthOfLastLine - span.iStartIndex;
                    } else {
                        indexDelta += es.LengthOfLastLine; // then delta is simply what we just inserted
                    }
                }
            }

            if (updateStart) {
                // Then start of selection is off the end of the list of edits.
                if (this.selection.iStartLine == currentLine) {
                    this.selection.iStartIndex += indexDelta;
                }
                this.selection.iStartLine += lineDelta;
            }
            if (updateEnd) {
                // Then end of selection is off the end of the list of edits.
                if (this.selection.iEndLine == currentLine) {
                    this.selection.iEndIndex += indexDelta;
                }
                this.selection.iEndLine += lineDelta;
            }
        }

        internal void ApplyEdits() {
            try {
                if (this.editList.Count == 0) return;

                if (this.changeCount != this.source.ChangeCount) {
                    throw new InvalidOperationException(SR.GetString(SR.BufferChanged));
                }

                if (this.view != null) {
                    using (cva) {
                        Apply();
                        cva.FlushEditActions(); // make sure text view has same coordinates
                    }
                    this.cva = null;
                    // Update selection.
                    this.view.SetSelection(this.selection.iStartLine, this.selection.iStartIndex, this.selection.iEndLine, this.selection.iEndIndex);
                    this.view.EnsureSpanVisible(this.selection);
                } else {
                    using (this.ca) {
                        Apply();
                    }
                    this.ca = null;
                }
            } finally {
                // If compound actions are not null then we need to abort them.
                Dispose();
            }
        }

        void Apply() {
            // use original edit spans to update the selection location so that we get the finest
            // grained selection update possible.  It should not be done on merged edits.
            ArrayList edits = this.editList;

            if (this.view != null) {
                // Now calculate the updated selection position based on what the edits are going to
                // do to the buffer.
                this.UpdateSelection(edits);
            }
            if (merge) {
                // Merge the edits into larger chunks for performance reasons.
                edits = this.MergeEdits(edits);
            }

            // Now apply the edits in reverse order because that one each edit will not interfere with the
            // span of the next edit.
            for (int i = edits.Count - 1; i >= 0; i--) {
                EditSpan es = (EditSpan)edits[i];
                TextSpan span = es.Span;
                string text = es.Text;
                this.source.SetText(span, text);
            }
            this.editList.Clear(); // done!
        }

        /// <summary>Allows enumeration of EditSpan objects</summary>
        public IEnumerator GetEnumerator() {
            return editList.GetEnumerator();
        }
        
    }

}
