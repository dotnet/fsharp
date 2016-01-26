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

using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    [Export(typeof(IViewTaggerProvider))]
    [TagType(typeof(IntraTextAdornmentTag))]
    [ContentType(ReplConstants.ReplContentTypeName)]
    internal class InlineReplAdornmentProvider : IViewTaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (buffer == null || textView == null || typeof(T) != typeof(IntraTextAdornmentTag)) {
                return null;
            }
            
            return (ITagger<T>)textView.Properties.GetOrCreateSingletonProperty<InlineReplAdornmentManager>(
                typeof(InlineReplAdornmentManager),
                () => new InlineReplAdornmentManager(textView)
                );
        }

        internal static InlineReplAdornmentManager GetManager(ITextView view) {
            InlineReplAdornmentManager result;
            if (!view.Properties.TryGetProperty<InlineReplAdornmentManager>(typeof(InlineReplAdornmentManager), out result)) {
                return null;
            }
            return result;
        }

        public static void AddInlineAdornment(ITextView view, UIElement uiElement, RoutedEventHandler onLoaded, SnapshotPoint targetLoc) {
            var manager = GetManager(view);
            if (manager != null) {
                var adornment = new ZoomableInlineAdornment(uiElement, view);
                // Event is unhooked in ReplWindow.  If we don't we'll receive the event multiple
                // times leading to very jerky / hang like behavior where we've setup a new event
                // loop in the repl window.
                adornment.Loaded += onLoaded;
                manager.AddAdornment(adornment, targetLoc);
            }
        }

        public static void ZoomInlineAdornments(ITextView view, double zoomFactor) {
            var manager = GetManager(view);
            if (manager != null) {
                foreach (var t in manager.Adornments) {
                    t.Item2.Zoom(zoomFactor);
                }
            }
        }

        public static void MinimizeLastInlineAdornment(ITextView view) {
            var manager = GetManager(view);
            if (manager != null && manager.Adornments.Count > 0) {
                var adornment = manager.Adornments[manager.Adornments.Count - 1].Item2;
                adornment.Zoom(adornment.MinimizedZoom);
            }
        }

        public static void RemoveAllAdornments(ITextView view) {
            var manager = GetManager(view);
            if (manager != null) {
                manager.RemoveAll();
            }
        }
    }
}
