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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Classifies regions for REPL error output spans.  These are always classified as errors.
    /// </summary>
    class ReplOutputClassifier : IClassifier {
        private readonly ReplOutputClassifierProvider _provider;
        internal static object ColorKey = new object();
        private readonly ITextBuffer _buffer;

        public ReplOutputClassifier(ReplOutputClassifierProvider provider, ITextBuffer buffer) {
            _provider = provider;
            _buffer = buffer;
        }

        #region IClassifier Members

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged {
            add { }
            remove { }
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            List<ColoredSpan> coloredSpans;
            if (!_buffer.Properties.TryGetProperty(ColorKey, out coloredSpans)) {
                return new ClassificationSpan[0];
            }

            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            int startIndex = coloredSpans.BinarySearch(new ColoredSpan(span, ConsoleColor.White), SpanStartComparer.Instance);
            if (startIndex == -1) {
                startIndex = 0;
            } else if (startIndex < 0) {
                startIndex = ~startIndex - 1;
            }

            int spanEnd = span.End.Position;
            for (int i = startIndex; i < coloredSpans.Count && coloredSpans[i].Span.Start < spanEnd; i++) {
                IClassificationType type;
                if (_provider._classTypes.TryGetValue(coloredSpans[i].Color, out type)) {
                    var overlap = span.Overlap(coloredSpans[i].Span);
                    if (overlap != null) {
                        classifications.Add(new ClassificationSpan(overlap.Value, type));
                    }
                }
            }

            return classifications;
        }

        private sealed class SpanStartComparer : IComparer<ColoredSpan> {
            internal static SpanStartComparer Instance = new SpanStartComparer();

            public int Compare(ColoredSpan x, ColoredSpan y) {
                return x.Span.Start - y.Span.Start;
            }
        }

        #endregion
    }
}
