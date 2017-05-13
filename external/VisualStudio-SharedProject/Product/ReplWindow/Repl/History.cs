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
using System.Collections.ObjectModel;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    internal class History {
        private readonly int _maxLength;
        private int _pos;
        private bool _live;
        private readonly List<HistoryEntry> _history;
        private string _uncommitedInput;

        internal History()
            : this(50) {
        }

        internal History(int maxLength) {
            _maxLength = maxLength;
            _pos = -1;
            _history = new List<HistoryEntry>();
        }

        internal void Clear() {
            _pos = -1;
            _live = false;
            _history.Clear();
        }

        internal int MaxLength {
            get { return _maxLength; }
        }

        internal int Length {
            get { return _history.Count; }
        }

        internal string UncommittedInput {
            get { return _uncommitedInput; }
            set { _uncommitedInput = value; }
        }

        internal IEnumerable<HistoryEntry> Items {
            get { return _history; }
        }

        internal HistoryEntry Last {
            get {
                if (_history.Count > 0) {
                    return _history[_history.Count - 1];
                } else {
                    return null;
                }
            }
        }

        internal void Add(string text) {
            var entry = new HistoryEntry { Text = text };
            _live = false;
            if (Length == 0 || Last.Text != text) {
                _history.Add(entry);
            }
            if (_history[InternalPosition].Text != text) {
                _pos = -1;
            }
            if (Length > MaxLength) {
                _history.RemoveAt(0);
                if (_pos > 0) {
                    _pos--;
                }
            }
        }

        private int InternalPosition {
            get {
                if (_pos == -1) {
                    return Length - 1;
                } else {
                    return _pos;
                }
            }
        }

        private string GetHistoryText(int pos) {
            if (pos < 0) {
                pos += Length;
            }
            return _history[pos].Text;
        }

        private string MoveToNext(string search) {
            do {
                _live = true;
                if (_pos < 0 || _pos == Length - 1) {
                    return null;
                }
                _pos++;
            } while (SearchDoesntMatch(search));

            return GetHistoryMatch(search);
        }
        
        private string MoveToPrevious(string search) {
            bool wasLive = _live;
            _live = true;
            do {
                if (Length == 0 || (Length > 1 && _pos == 0)) {
                    // we have no history or we have history but have scrolled to the very beginning
                    return null;
                }
                if (_pos == -1) {
                    // no search in progress, start our search at the end
                    _pos = Length - 1;
                } else if (!wasLive && String.IsNullOrWhiteSpace(search)) {
                    // Handles up up up enter up
                    // Do nothing
                } else {
                    // go to the previous item
                    _pos--;
                }
            } while (SearchDoesntMatch(search));

            return GetHistoryMatch(search);
        }

        private bool SearchDoesntMatch(string search) {
            return !String.IsNullOrWhiteSpace(search) && GetHistoryText(_pos).IndexOf(search) == -1;
        }

        private string GetHistoryMatch(string search) {
            if (SearchDoesntMatch(search)) {
                return null;
            }

            return GetHistoryText(_pos);
        }

        private string Get(Func<string, string> moveFn, string search) {
            var startPos = _pos;
            string next = moveFn(search);
            if (next == null) {
                _pos = startPos;
                return null;
            }
            return next;
        }

        internal string GetNext(string search = null) {
            return Get(MoveToNext, search);
        }

        internal string GetPrevious(string search = null) {
            return Get(MoveToPrevious, search);
        }

        internal class HistoryEntry {
            internal string Text;
            internal bool Command;
            internal int Duration;
            internal bool Failed;
        }
    }
}
