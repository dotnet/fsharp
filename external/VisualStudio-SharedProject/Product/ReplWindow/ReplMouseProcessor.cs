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
#if NTVS_FEATURE_INTERACTIVEWINDOW
using Microsoft.NodejsTools.Repl;
#else
using Microsoft.VisualStudio.Repl;
#endif
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools {
#else
namespace Microsoft.VisualStudio {
#endif
    /// <summary>
    /// Processes right click events in the REPL window to handle our adornment
    /// context menu.
    /// </summary>
    [Export(typeof(IMouseProcessorProvider))]
    [Name("ReplWindowMouseProcessor")]
    [Order(Before = "VisualStudioMouseProcessor")]
    [ContentType("text")] // or whatever your adornment is specific to
    [TextViewRole(ReplConstants.ReplTextViewRole)]
    internal sealed class ReplMouseProcessorProvider : IMouseProcessorProvider {

        #region IMouseProcessorProvider Members

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView) {
            return new MouseProcessor(wpfTextView);
        }

        #endregion

        class MouseProcessor : IMouseProcessor {
            private readonly IWpfTextView _view;

            public MouseProcessor(IWpfTextView view) {
                _view = view;
            }

            #region IMouseProcessor Members

            public void PostprocessDragEnter(System.Windows.DragEventArgs e) {
            }

            public void PostprocessDragLeave(System.Windows.DragEventArgs e) {
            }

            public void PostprocessDragOver(System.Windows.DragEventArgs e) {
            }

            public void PostprocessDrop(System.Windows.DragEventArgs e) {
            }

            public void PostprocessGiveFeedback(System.Windows.GiveFeedbackEventArgs e) {
            }

            public void PostprocessMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseEnter(System.Windows.Input.MouseEventArgs e) {
            }

            public void PostprocessMouseLeave(System.Windows.Input.MouseEventArgs e) {
            }

            public void PostprocessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseMove(System.Windows.Input.MouseEventArgs e) {
            }

            public void PostprocessMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PostprocessMouseWheel(System.Windows.Input.MouseWheelEventArgs e) {
            }

            public void PostprocessQueryContinueDrag(System.Windows.QueryContinueDragEventArgs e) {
            }

            public void PreprocessDragEnter(System.Windows.DragEventArgs e) {
            }

            public void PreprocessDragLeave(System.Windows.DragEventArgs e) {
            }

            public void PreprocessDragOver(System.Windows.DragEventArgs e) {
            }

            public void PreprocessDrop(System.Windows.DragEventArgs e) {
            }

            public void PreprocessGiveFeedback(System.Windows.GiveFeedbackEventArgs e) {
            }

            public void PreprocessMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PreprocessMouseEnter(System.Windows.Input.MouseEventArgs e) {
            }

            public void PreprocessMouseLeave(System.Windows.Input.MouseEventArgs e) {
            }

            public void PreprocessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PreprocessMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PreprocessMouseMove(System.Windows.Input.MouseEventArgs e) {
            }

            public void PreprocessMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PreprocessMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
                var manager = InlineReplAdornmentProvider.GetManager(_view);
                if (manager != null) {
                    var over = System.Windows.Input.Mouse.PrimaryDevice.DirectlyOver as System.Windows.FrameworkElement;
                    while (over != null) {
                        ZoomableInlineAdornment adornment = over as ZoomableInlineAdornment;
                        if(adornment != null) {
                            adornment.ContextMenu.IsOpen = true;
                            e.Handled = true;
                            break;

                        }
                        over = over.Parent as System.Windows.FrameworkElement;
                    }
                    
                }
            }

            public void PreprocessMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            }

            public void PreprocessMouseWheel(System.Windows.Input.MouseWheelEventArgs e) {
            }

            public void PreprocessQueryContinueDrag(System.Windows.QueryContinueDragEventArgs e) {
            }

            #endregion
        }
    }
}
