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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    internal sealed class PromptMarginVisualManager {
        private readonly PromptMargin _margin;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly Canvas _canvas;
        private readonly Grid _glyphMarginGrid;
        private Dictionary<UIElement, GlyphData> _glyphs;

        public PromptMarginVisualManager(PromptMargin margin, IEditorFormatMap editorFormatMap) {
            _margin = margin;

            _editorFormatMap = editorFormatMap;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            margin.TextView.Closed += OnTextViewClosed;

            _glyphs = new Dictionary<UIElement, GlyphData>();

            _glyphMarginGrid = new Grid();
            _glyphMarginGrid.Width = 17.0;

            UpdateBackgroundColor();
            
            Canvas canvas = new Canvas();
            canvas.Background = Brushes.Transparent;
            canvas.ClipToBounds = true;

            _glyphMarginGrid.Children.Add(canvas);
            _canvas = canvas;
        }

        public void RemoveGlyphsByVisualSpan(SnapshotSpan span) {
            List<UIElement> glyphsInSpan = new List<UIElement>();

            foreach (var glyph in _glyphs) {
                GlyphData data = glyph.Value;

                if (data.VisualSpan.HasValue) {
                    if (span.IntersectsWith(data.VisualSpan.Value)) {
                        glyphsInSpan.Add(glyph.Key);
                        _canvas.Children.Remove(data.Element);
                    }
                }
            }
            foreach (UIElement element in glyphsInSpan) {
                _glyphs.Remove(element);
            }
        }

        public void AddGlyph(string text, SnapshotSpan span) {
            IWpfTextViewLineCollection lines = _margin.TextView.TextViewLines;

            bool visible = _margin.TextView.TextViewLines.IntersectsBufferSpan(span);
            if (visible) {
                ITextViewLine line = GetStartingLine(lines, span, returnLastLine: true);
                if (line != null) {
                    UIElement element = CreatePromptElement(text);

                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double leftPlacement = (17.0 - element.DesiredSize.Width) / 2;
                    Canvas.SetLeft(element, leftPlacement);
                    GlyphData data = new GlyphData(span, element);
                    data.SetTop(line.TextTop - _margin.TextView.ViewportTop);

                    _glyphs[element] = data;
                    _canvas.Children.Add(element);
                }
            }
        }

        private UIElement CreatePromptElement(string text) {
            TextBlock block = new TextBlock();
            block.Text = text;
            block.Foreground = _margin.PromptProvider.HostControl.Foreground;
            block.FontSize = _margin.PromptProvider.HostControl.FontSize;
            block.FontFamily = _Consolas; // TODO: get the font family from the editor?
            return block;
        }

        private static readonly FontFamily _Consolas = new FontFamily("Consolas");

        public void SetSnapshotAndUpdate(ITextSnapshot snapshot, IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines) {
            if (_glyphs.Count > 0) {
                // Go through all the existing visuals and invalidate or transform as appropriate.
                Dictionary<UIElement, GlyphData> newVisuals = new Dictionary<UIElement, GlyphData>(_glyphs.Count);

                foreach (var glyph in _glyphs) {
                    GlyphData data = glyph.Value;

                    if (!data.VisualSpan.HasValue) {
                        newVisuals[glyph.Key] = data;
                    } else {
                        data.SetSnapshot(snapshot);

                        SnapshotSpan span = data.VisualSpan.Value;

                        if ((!_margin.TextView.TextViewLines.IntersectsBufferSpan(span)) ||
                            (GetStartingLine(newOrReformattedLines, span, returnLastLine: false) != null)) {
                            //Either visual is no longer visible or it crosses a line
                            //that was reformatted.

                            _canvas.Children.Remove(data.Element);
                        } else {
                            newVisuals[data.Element] = data;
                            ITextViewLine line = GetStartingLine(translatedLines, span, returnLastLine: true);
                            if (line != null) {
                                data.SetTop(line.Top - _margin.TextView.ViewportTop);
                            }
                        }
                    }
                }

                _glyphs = newVisuals;
            }
        }

        public FrameworkElement MarginVisual {
            get { return _glyphMarginGrid; }
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e) {
            if (e.ChangedItems.Contains("Indicator Margin")) {
                UpdateBackgroundColor();
            }
        }

        private void OnTextViewClosed(object sender, EventArgs e) {
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
        }

        private void UpdateBackgroundColor() {
            ResourceDictionary resourceDictionary = _editorFormatMap.GetProperties("Indicator Margin");
            if (resourceDictionary.Contains(EditorFormatDefinition.BackgroundColorId)) {
                _glyphMarginGrid.Background = new SolidColorBrush((Color)resourceDictionary[EditorFormatDefinition.BackgroundColorId]);
            } else if (resourceDictionary.Contains(EditorFormatDefinition.BackgroundBrushId)) {
                _glyphMarginGrid.Background = (Brush)resourceDictionary[EditorFormatDefinition.BackgroundBrushId];
            } else {
                _glyphMarginGrid.Background = Brushes.Transparent;
            }
        }

        internal static ITextViewLine GetStartingLine(IList<ITextViewLine> lines, Span span, bool returnLastLine) {
            if (lines.Count > 0) {
                int low = 0;
                int high = lines.Count;
                while (low < high) {
                    int middle = (low + high) / 2;
                    ITextViewLine middleLine = lines[middle];
                    if (span.Start < middleLine.Start)
                        high = middle;
                    else if (span.Start >= middleLine.EndIncludingLineBreak)
                        low = middle + 1;
                    else
                        return middleLine;
                }

                if (returnLastLine) {
                    ITextViewLine lastLine = lines[lines.Count - 1];
                    if ((lastLine.EndIncludingLineBreak == lastLine.Snapshot.Length) && (span.Start == lastLine.EndIncludingLineBreak)) {
                        // As a special case, if the last line ends at the end of the buffer and the span starts at the end of the buffer
                        // as well, treat is as crossing the last line in the buffer.
                        return lastLine;
                    }
                }
            }

            return null;
        }
        
        private sealed class GlyphData {
            private readonly UIElement _element;
            private SnapshotSpan? _visualSpan;
            internal double _deltaY;

            public GlyphData(SnapshotSpan visualSpan, UIElement element) {
                _visualSpan = visualSpan;
                _element = element;

                _deltaY = Canvas.GetTop(element);
                if (double.IsNaN(_deltaY))
                    _deltaY = 0.0;
            }

            public SnapshotSpan? VisualSpan { get { return _visualSpan; } }
            public UIElement Element { get { return _element; } }

            public void SetSnapshot(ITextSnapshot snapshot) {
                _visualSpan = _visualSpan.Value.TranslateTo(snapshot, SpanTrackingMode.EdgeInclusive);
            }

            public void SetTop(double top) {
                Canvas.SetTop(_element, top + _deltaY);
            }
        }
    }
}
