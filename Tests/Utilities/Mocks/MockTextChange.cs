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
using Microsoft.VisualStudio.Text;

namespace TestUtilities.Mocks {
    class MockTextChange : ITextChange {
        private readonly SnapshotSpan _removed;
        private readonly string _inserted;
        private readonly int _newStart;
        private static readonly string[] NewLines = new[] { "\r\n", "\r", "\n"};

        public MockTextChange(SnapshotSpan removedSpan, int newStart, string insertedText) {
            _removed = removedSpan;
            _inserted = insertedText;
            _newStart = newStart;
        }

        public int Delta {
            get { return _inserted.Length - _removed.Length; }
        }

        public int LineCountDelta {
            get {
                return _inserted.Split(NewLines, StringSplitOptions.None).Length -
                    _removed.GetText().Split(NewLines, StringSplitOptions.None).Length;
            }
        }

        public int NewEnd {
            get {
                return NewPosition + _inserted.Length;
            }
        }

        public int NewLength {
            get { return _inserted.Length; }
        }

        public int NewPosition {
            get { return _newStart; }
        }

        public Span NewSpan {
            get {
                return new Span(NewPosition, NewLength);
            }
        }

        public string NewText {
            get { return _inserted; }
        }

        public int OldEnd {
            get { return _removed.End; }
        }

        public int OldLength {
            get { return _removed.Length; }
        }

        public int OldPosition {
            get { return _removed.Start; }
        }

        public Span OldSpan {
            get { return _removed.Span; }
        }

        public string OldText {
            get { return _removed.GetText(); }
        }
    }
}
