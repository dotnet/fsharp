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
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using System.Diagnostics;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(PromptMargin.MarginName)]
    [Order(Before = PredefinedMarginNames.LeftSelection)]
    [MarginContainer(PredefinedMarginNames.Left)]
    [ContentType(ReplConstants.ReplContentTypeName)]
    [TextViewRole(ReplConstants.ReplTextViewRole)]
    internal sealed class PromptMarginProvider : IWpfTextViewMarginProvider {
        [Import]
        internal IEditorFormatMapService EditorFormatMapService;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin parent) {
            return new PromptMargin(wpfTextViewHost, EditorFormatMapService.GetEditorFormatMap(wpfTextViewHost.TextView));
        }
    }

    /// <summary>
    /// Provides glyphs corresponding to GlyphTags in the buffer.
    /// </summary>
    internal sealed class PromptMargin : IWpfTextViewMargin {
#if NTVS_FEATURE_INTERACTIVEWINDOW
        public const string MarginName = "NodejsInteractivePromptMargin";
#else
        public const string MarginName = "InteractivePromptMargin";
#endif

        private readonly IWpfTextView _textView;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly ReplWindow _promptProvider;
        private PromptMarginVisualManager _visualManager;

        public PromptMargin(IWpfTextViewHost wpfTextViewHost, IEditorFormatMap editorFormatMap) {
            _textView = wpfTextViewHost.TextView;
            _editorFormatMap = editorFormatMap;

            _promptProvider = ReplWindow.FromBuffer(_textView.TextBuffer);
            _promptProvider.MarginVisibilityChanged += new Action(OnMarginVisibilityChanged);

            _visualManager = new PromptMarginVisualManager(this, editorFormatMap);
            _visualManager.MarginVisual.IsVisibleChanged += this.OnIsVisibleChanged;

            OnMarginVisibilityChanged();
        }

        internal IWpfTextView TextView
        {
            get { return _textView; }
        }

        internal ReplWindow PromptProvider {
            get { return _promptProvider; }
        }

        void OnMarginVisibilityChanged() {
            _visualManager.MarginVisual.Visibility = this.Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                _textView.LayoutChanged += OnLayoutChanged;
                _textView.ZoomLevelChanged += OnZoomLevelChanged;

                foreach (var line in _textView.TextViewLines) {
                    RefreshGlyphsOver(line);
                }

                _visualManager.MarginVisual.LayoutTransform = new ScaleTransform(scaleX: _textView.ZoomLevel / 100, scaleY: _textView.ZoomLevel / 100);
                if (_visualManager.MarginVisual.LayoutTransform.CanFreeze) {
                    _visualManager.MarginVisual.LayoutTransform.Freeze();
                }
            } else {
                _visualManager.RemoveGlyphsByVisualSpan(new SnapshotSpan(_textView.TextSnapshot, 0, _textView.TextSnapshot.Length));

                _textView.LayoutChanged -= OnLayoutChanged;
                _textView.ZoomLevelChanged -= OnZoomLevelChanged;
            }
        }

        void OnZoomLevelChanged(object sender, ZoomLevelChangedEventArgs e) {
            _visualManager.MarginVisual.LayoutTransform = e.ZoomTransform;
        }

        void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            // If the view did a vertical translation then we can generally assume the screen position of every line changed.
            _visualManager.SetSnapshotAndUpdate(
                _textView.TextSnapshot, 
                e.NewOrReformattedLines, 
                e.VerticalTranslation ? ((IList<ITextViewLine>)_textView.TextViewLines) : e.TranslatedLines
            );

            foreach (ITextViewLine line in e.NewOrReformattedLines) {
                _visualManager.RemoveGlyphsByVisualSpan(line.Extent);
                RefreshGlyphsOver(line);
            }
        }

        private void RefreshGlyphsOver(ITextViewLine textViewLine) {
            foreach (var prompt in _promptProvider.GetOverlappingPrompts(textViewLine.Extent)) {
                SnapshotSpan span = new SnapshotSpan(prompt.Value, 0);
                ReplSpanKind kind = prompt.Key;

                if (textViewLine.End == prompt.Value || textViewLine.Extent.Contains(prompt.Value)) {
                    _visualManager.AddGlyph(_promptProvider.GetPromptText(kind), span);
                }
            }
        }

        private void ThrowIfDisposed() {
            if (_visualManager == null) {
                throw new ObjectDisposedException(PredefinedMarginNames.Glyph);
            }
        }

        #region IWpfTextViewMargin Members

        public FrameworkElement VisualElement {
            get {
                ThrowIfDisposed();
                return _visualManager.MarginVisual;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize {
            get {
                ThrowIfDisposed();
                return _visualManager.MarginVisual.Width;
            }
        }

        public bool Enabled {
            get {
                ThrowIfDisposed();
                return _promptProvider.DisplayPromptInMargin;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName) {
            return string.Compare(marginName, MarginName, StringComparison.OrdinalIgnoreCase) == 0 ? this : (ITextViewMargin)null;
        }

        public void Dispose() {
            if (_promptProvider != null) {
                _promptProvider.MarginVisibilityChanged -= this.OnMarginVisibilityChanged;
            }

            if (_visualManager != null) {
                var visual = _visualManager.MarginVisual;
                if (visual != null) {
                    visual.IsVisibleChanged -= this.OnIsVisibleChanged;
                }
            }

            _visualManager = null;
        }

        #endregion
    }
}
