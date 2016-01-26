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
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    class InlineReplAdornmentManager : ITagger<IntraTextAdornmentTag> {
        private readonly ITextView _textView;
        private readonly List<Tuple<SnapshotPoint, ZoomableInlineAdornment>> _tags;
        private readonly Dispatcher _dispatcher;

        internal InlineReplAdornmentManager(ITextView textView) {
            _textView = textView;
            _tags = new List<Tuple<SnapshotPoint, ZoomableInlineAdornment>>();
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            var result = new List<TagSpan<IntraTextAdornmentTag>>();
            for (int i = 0; i < _tags.Count; i++) {
                if (_tags[i].Item1.Snapshot != _textView.TextSnapshot) {
                    // update to the latest snapshot
                    _tags[i] = new Tuple<SnapshotPoint, ZoomableInlineAdornment>(
                        _tags[i].Item1.TranslateTo(_textView.TextSnapshot, PointTrackingMode.Negative),
                        _tags[i].Item2
                    );
                }
                
                var span = new SnapshotSpan(_textView.TextSnapshot, _tags[i].Item1, 0);
                bool intersects = false;
                foreach (var applicableSpan in spans) {
                    if (applicableSpan.TranslateTo(_textView.TextSnapshot, SpanTrackingMode.EdgeInclusive).IntersectsWith(span)) {
                        intersects = true;
                        break;
                    }
                }
                if (!intersects) {
                    continue;
                }
                var tag = new IntraTextAdornmentTag(_tags[i].Item2, null);
                result.Add(new TagSpan<IntraTextAdornmentTag>(span, tag));
            }
            return result;
        }

        public void AddAdornment(ZoomableInlineAdornment uiElement, SnapshotPoint targetLoc) {
            if (Dispatcher.CurrentDispatcher != _dispatcher) {
                _dispatcher.BeginInvoke(new Action(() => AddAdornment(uiElement, targetLoc)));
                return;
            }
            var targetLine = targetLoc.GetContainingLine();
            _tags.Add(new Tuple<SnapshotPoint, ZoomableInlineAdornment>(targetLoc, uiElement));
            var handler = TagsChanged;
            if (handler != null) {
                var span = new SnapshotSpan(_textView.TextSnapshot, targetLine.Start, targetLine.LengthIncludingLineBreak);
                var args = new SnapshotSpanEventArgs(span);
                handler(this, args);
            }
        }

        public IList<Tuple<SnapshotPoint, ZoomableInlineAdornment>> Adornments {
            get { return _tags; }
        }

        public void RemoveAll() {
            _tags.Clear();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
