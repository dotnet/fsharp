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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    internal class ZoomableInlineAdornment : ContentControl {
        private readonly ITextView _parent;
        private ResizingAdorner _adorner;
        private bool _isResizing;
        private double _zoom;
        private readonly double _minimizedZoom;
        private readonly double _zoomStep;
        private readonly double _widthRatio, _heightRatio;

        public ZoomableInlineAdornment(UIElement content, ITextView parent) {
            _parent = parent;
            Debug.Assert(parent is IInputElement);
            Content = new Border { BorderThickness = new Thickness(1), Child = content, Focusable = true };

            _zoom = 1.0;             // config.GetConfig().Repl.InlineMedia.MaximizedZoom
            _zoomStep = 0.25;        // config.GetConfig().Repl.InlineMedia.ZoomStep
            _minimizedZoom = 0.25;   // config.GetConfig().Repl.InlineMedia.MinimizedZoom
            _widthRatio = 0.67;      // config.GetConfig().Repl.InlineMedia.WidthRatio
            _heightRatio = 0.5;      // config.GetConfig().Repl.InlineMedia.HeightRatio

            _isResizing = false;
            UpdateSize();

            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;

            ContextMenu = MakeContextMenu();

            var trigger = new Trigger { Property = Border.IsFocusedProperty, Value = true };
            var setter = new Setter { Property = Border.BorderBrushProperty, Value = SystemColors.ActiveBorderBrush };
            trigger.Setters.Add(setter);

            var style = new Style();
            style.Triggers.Add(trigger);
            MyContent.Style = style;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonDown(e);

            ((Border)Content).Focus();
            e.Handled = true;
        }

        private ContextMenu MakeContextMenu() {
            var result = new ContextMenu();
            AddMenuItem(result, "Zoom In", "Ctrl+OemPlus", (s, e) => OnZoomIn());
            AddMenuItem(result, "Zoom Out", "Ctrl+OemMinus", (s, e) => OnZoomOut());
            result.Items.Add(new Separator());
            AddMenuItem(result, "150%", null, (s, e) => Zoom(1.5));
            AddMenuItem(result, "100%", null, (s, e) => Zoom(1.0));
            AddMenuItem(result, "75%", null, (s, e) => Zoom(0.75));
            AddMenuItem(result, "50%", null, (s, e) => Zoom(0.50));
            AddMenuItem(result, "25%", null, (s, e) => Zoom(0.25));
            return result;
        }

        private static void AddMenuItem(ContextMenu menu, string text, string shortcut, EventHandler handler) {
            var item = new MenuItem();
            item.Header = text;
            item.Click += (s, e) => handler(s, e);
            menu.Items.Add(item);
        }

        private Border MyContent {
            get { return Content as Border; }
        }

        private void OnGotFocus(object sender, RoutedEventArgs args) {
            _adorner = new ResizingAdorner(MyContent);
            _adorner.ResizeStarted += OnResizeStarted;
            _adorner.ResizeCompleted += OnResizeCompleted;

            var adornerLayer = AdornerLayer.GetAdornerLayer(MyContent);
            if (adornerLayer != null) {
                adornerLayer.Add(_adorner);
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs args) {
            _adorner.ResizeStarted -= OnResizeStarted;
            _adorner.ResizeCompleted -= OnResizeCompleted;

            var adornerLayer = AdornerLayer.GetAdornerLayer(MyContent);
            if (adornerLayer != null) {
                adornerLayer.Remove(_adorner);
                _adorner = null;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs args) {
            var modifiers = args.KeyboardDevice.Modifiers & ModifierKeys.Control;
            if (modifiers == ModifierKeys.Control && (args.Key == Key.OemPlus || args.Key == Key.Add)) {
                OnZoomIn();
                args.Handled = true;
            } else if (modifiers == ModifierKeys.Control && (args.Key == Key.OemMinus || args.Key == Key.Subtract)) {
                OnZoomOut();
                args.Handled = true;
            }

            base.OnPreviewKeyDown(args);
        }

        private void OnResizeStarted(object sender, RoutedEventArgs args) {
            _isResizing = true;
        }

        private void OnResizeCompleted(object sender, RoutedEventArgs args) {
            _isResizing = false;
            _zoom = MyContent.DesiredSize.Width / (_parent.ViewportWidth * _widthRatio);
        }

        internal void Zoom(double zoomFactor) {
            _zoom = zoomFactor;
            UpdateSize();
        }

        private void OnZoomIn() {
            _zoom += _zoomStep;
            UpdateSize();
        }

        private void OnZoomOut() {
            if (_zoom - _zoomStep > 0.1) {
                _zoom -= _zoomStep;
                UpdateSize();
            }
        }

        internal void UpdateSize() {
            if (_isResizing) {
                return;
            }

            double width = _parent.ViewportWidth * _widthRatio * _zoom;
            double height = _parent.ViewportHeight * _heightRatio * _zoom;
            MyContent.MaxWidth = width;
            MyContent.MaxHeight = height;
            MyContent.Measure(new Size(width, height));
        }

        internal double MinimizedZoom {
            get { return _minimizedZoom; }
        }
    }
}
