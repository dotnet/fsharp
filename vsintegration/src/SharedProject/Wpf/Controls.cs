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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudioTools.Wpf {
    public static class Controls {
        public static readonly object BackgroundKey = VsBrushes.WindowKey;
        public static readonly object BackgroundColorKey = VsColors.WindowKey;
        public static readonly object BackgroundAccentKey = VsBrushes.ButtonFaceKey;
        public static readonly object ForegroundKey = VsBrushes.WindowTextKey;
        public static readonly object GrayTextKey = VsBrushes.GrayTextKey;
        public static readonly object HighlightKey = VsBrushes.HighlightKey;
        public static readonly object HighlightTextKey = VsBrushes.HighlightTextKey;
        public static readonly object HotTrackKey = VsBrushes.CommandBarMouseOverBackgroundGradientKey;

        public static readonly object TooltipBackgroundKey = VsBrushes.InfoBackgroundKey;
        public static readonly object TooltipTextKey = VsBrushes.InfoTextKey;

        public static readonly object HyperlinkKey = VsBrushes.ControlLinkTextKey;
        public static readonly object HyperlinkHoverKey = VsBrushes.ControlLinkTextHoverKey;

        public static readonly object ControlBackgroundKey = VsBrushes.ComboBoxBackgroundKey;
        public static readonly object ControlForegroundKey = VsBrushes.WindowTextKey;
        public static readonly object ControlBorderKey = VsBrushes.ComboBoxBorderKey;
        public static readonly object ControlBackgroundHoverKey = VsBrushes.ComboBoxMouseOverBackgroundGradientKey;
        public static readonly object ControlBorderHoverKey = VsBrushes.ComboBoxMouseOverGlyphKey;
        public static readonly object ControlBackgroundPressedKey = VsBrushes.ComboBoxMouseDownBackgroundKey;
        public static readonly object ControlForegroundPressedKey = VsBrushes.ComboBoxGlyphKey;
        public static readonly object ControlBorderPressedKey = VsBrushes.ComboBoxMouseDownBorderKey;
        public static readonly object ControlBackgroundSelectedKey = VsBrushes.ComboBoxMouseDownBackgroundKey;
        public static readonly object ControlForegroundSelectedKey = VsBrushes.ComboBoxGlyphKey;
        public static readonly object ControlBorderSelectedKey = VsBrushes.ComboBoxMouseOverBorderKey;
        public static readonly object ControlBackgroundDisabledKey = VsBrushes.ComboBoxDisabledBackgroundKey;
        public static readonly object ControlForegroundDisabledKey = VsBrushes.ComboBoxDisabledGlyphKey;
        public static readonly object ControlBorderDisabledKey = VsBrushes.ComboBoxDisabledBorderKey;

        public static readonly object ComboBoxPopupBackgroundKey = VsBrushes.ComboBoxPopupBackgroundGradientKey;
        public static readonly object ComboBoxPopupBorderKey = VsBrushes.ComboBoxPopupBorderKey;
        public static readonly object ComboBoxPopupForegroundKey = VsBrushes.WindowTextKey;

        public static readonly object ButtonForegroundPressedKey = VsBrushes.ToolWindowButtonDownActiveGlyphKey;
        public static readonly object ButtonBackgroundPressedKey = VsBrushes.ComboBoxMouseOverBorderKey;
        public static readonly object ButtonBackgroundHoverKey = VsBrushes.CommandBarHoverOverSelectedKey;
        public static readonly object ButtonBorderHoverKey = VsBrushes.ComboBoxMouseOverGlyphKey;

        public static readonly object ScrollBarBackgroundKey = VsBrushes.ScrollBarBackgroundKey;
        public static readonly object ScrollBarThumbBackgroundKey = VsBrushes.ScrollBarThumbBackgroundKey;
        public static readonly object ScrollBarThumbBackgroundHoverKey = VsBrushes.ScrollBarThumbMouseOverBackgroundKey;
        public static readonly object ScrollBarThumbBackgroundPressedKey = VsBrushes.ScrollBarThumbPressedBackgroundKey;
        public static readonly object ScrollBarArrowKey = VsBrushes.ScrollBarThumbGlyphKey;
        public static readonly object ScrollBarArrowHoverKey = VsBrushes.GrayTextKey;
        public static readonly object ScrollBarArrowPressedKey = VsBrushes.WindowTextKey;
        public static readonly object ScrollBarArrowDisabledKey = VsBrushes.ScrollBarThumbGlyphKey;
        public static readonly object ScrollBarArrowBackgroundKey = VsBrushes.ScrollBarArrowBackgroundKey;
        public static readonly object ScrollBarArrowBackgroundHoverKey = VsBrushes.ScrollBarArrowMouseOverBackgroundKey;
        public static readonly object ScrollBarArrowBackgroundPressedKey = VsBrushes.ScrollBarArrowPressedBackgroundKey;
        public static readonly object ScrollBarArrowBackgroundDisabledKey = VsBrushes.ScrollBarArrowDisabledBackgroundKey;

#if DEV11_OR_LATER
        public static readonly object SearchGlyphBrushKey = SearchControlColors.SearchGlyphBrushKey;
#else
        public static readonly object SearchGlyphBrushKey = VsBrushes.WindowTextKey;
#endif

        public static readonly BitmapSource UacShield = CreateUacShield();

        private static BitmapSource CreateUacShield() {
            if (Environment.OSVersion.Version.Major >= 6) {
                var sii = new NativeMethods.SHSTOCKICONINFO();
                sii.cbSize = (UInt32)Marshal.SizeOf(typeof(NativeMethods.SHSTOCKICONINFO));

                Marshal.ThrowExceptionForHR(NativeMethods.SHGetStockIconInfo(77, 0x0101, ref sii));
                try {
                    return Imaging.CreateBitmapSourceFromHIcon(
                        sii.hIcon,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                } finally {
                    NativeMethods.DestroyIcon(sii.hIcon);
                }
            } else {
                return Imaging.CreateBitmapSourceFromHIcon(
                    SystemIcons.Shield.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
