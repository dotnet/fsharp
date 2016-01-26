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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace TestUtilities.Mocks {
    public class MockTextEdit : ITextEdit {
        private readonly List<Edit> _edits = new List<Edit>();
        private readonly MockTextSnapshot _snapshot;
        private bool _canceled, _applied;

        public MockTextEdit(MockTextSnapshot snapshot) {
            _snapshot = snapshot;
        }

        public bool Delete(int startPosition, int charsToDelete) {
            _edits.Add(new DeletionEdit(startPosition, charsToDelete));
            return true;
        }

        public bool Delete(Span deleteSpan) {
            return Delete(deleteSpan.Start, deleteSpan.Length);
        }

        public bool HasEffectiveChanges {
            get { throw new System.NotImplementedException(); }
        }

        public bool HasFailedChanges {
            get { throw new System.NotImplementedException(); }
        }

        public bool Insert(int position, char[] characterBuffer, int startIndex, int length) {
            return Insert(position, new String(characterBuffer, startIndex, length));
        }

        public bool Insert(int position, string text) {
            _edits.Add(new InsertionEdit(position, text));
            return true;
        }

        public bool Replace(int startPosition, int charsToReplace, string replaceWith) {
            Delete(startPosition, charsToReplace);
            Insert(startPosition, replaceWith);
            return true;
        }

        public bool Replace(Span replaceSpan, string replaceWith) {
            return Replace(replaceSpan.Start, replaceSpan.Length, replaceWith);
        }

        private static int CompareEdits(Edit left, Edit right) {
            int res = right.Position - left.Position;
            if (res == 0) {
                if (left is InsertionEdit) {
                    if (right is DeletionEdit) {
                        res = 1;
                    }
                } else if (right is InsertionEdit) {
                    res = -1;
                }
            }
            return res;
        }

        public ITextSnapshot Apply() {
            StringBuilder text = new StringBuilder(_snapshot.GetText());
            var deletes = new NormalizedSnapshotSpanCollection(
                _snapshot,
                _edits.Where(edit => edit is DeletionEdit)
                .Select(edit =>
                    new Span(
                        ((DeletionEdit)edit).Position,
                        ((DeletionEdit)edit).Length
                    )
                )
            );

            // apply the deletes
            for (int i = deletes.Count - 1; i >= 0; i--) {
                text.Remove(deletes[i].Start, deletes[i].Length);
            }

            // now apply the inserts
            int curDelete = 0, adjust = 0;
            int deletesBorrowed = 0;
            foreach (InsertionEdit insert in _edits.Where(edit => edit is InsertionEdit)) {
                while (curDelete < deletes.Count && deletes[curDelete].Start < insert.Position) {
                    if (deletes[curDelete].Start + deletes[curDelete].Length < insert.Position) {
                        adjust -= deletes[curDelete].Length - deletesBorrowed;
                        deletesBorrowed = 0;
                        curDelete++;
                    } else {
                        int deletesUsed = insert.Position - deletes[curDelete].Start;
                        adjust -= deletesUsed;
                        deletesBorrowed  += deletesUsed;
                        break;
                    }
                }

                text.Insert(insert.Position + adjust, insert.Text);
                adjust += insert.Text.Length;
            }

            List<MockTextChange> changes = new List<MockTextChange>();
            for (int i = 0; i < _edits.Count; i++) {
                var curEdit = _edits[i];
                InsertionEdit insert = curEdit as InsertionEdit;
                if (insert != null) {
                    changes.Add(
                        new MockTextChange(
                            new SnapshotSpan(
                                _snapshot,
                                insert.Position,
                                0
                            ),
                            insert.Position,
                            insert.Text
                        )
                    );
                } else {
                    DeletionEdit delete = curEdit as DeletionEdit;
                    changes.Add(
                        new MockTextChange(
                            new SnapshotSpan(
                                _snapshot,
                                delete.Position,
                                delete.Length
                            ),
                            delete.Position + adjust,
                            ""
                        )
                    );
                }
            }

            var previous = _snapshot;
            var res = ((MockTextBuffer)_snapshot.TextBuffer)._snapshot = new MockTextSnapshot(
                (MockTextBuffer)_snapshot.TextBuffer,
                text.ToString(),
                _snapshot,
                changes.ToArray()
            );
            _applied = true;
            ((MockTextBuffer)_snapshot.TextBuffer).EditApplied(previous);
            return res;
        }

        public void Cancel() {
            _edits.Clear();
            _canceled = true;
        }

        public bool Canceled {
            get { return _canceled; }
        }

        public ITextSnapshot Snapshot {
            get { return _snapshot; }
        }

        public void Dispose() {
            if (!_applied) {
                Cancel();
            }
        }

        class Edit {
            public readonly int Position;

            public Edit(int position) {
                Position = position;
            }
        }

        sealed class InsertionEdit : Edit {
            public readonly string Text;

            public InsertionEdit(int position, string text)
                : base(position) {
                Text = text;
            }

            public override string ToString() {
                return String.Format("<Insert Length={0} at {1}>", Text.Length, Position);
            }
        }

        sealed class DeletionEdit : Edit {
            public readonly int Length;

            public DeletionEdit(int startPosition, int charsToDelete)
                : base(startPosition) {
                Length = charsToDelete;
            }

            public override string ToString() {
                return String.Format("<Delete Length={0} at {1}>", Length, Position);
            }
        }
    }
}
