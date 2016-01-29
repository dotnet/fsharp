// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace Microsoft.VisualStudio.FSharp.LanguageService
{

    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class Colorizer : IVsColorizer
    {
        int suspended;

        internal Colorizer() { }

        public abstract void CloseColorizer();

        public virtual int GetStateMaintenanceFlag(out int flag)
        {
            flag = 1;
            return NativeMethods.S_OK;
        }

        public abstract int GetStartState(out int start);
        public abstract int GetStateAtEndOfLine(int line, int length, IntPtr ptr, int state);
        public abstract int ColorizeLine(int line, int length, IntPtr ptr, int state, uint[] attrs);
        internal abstract TokenInfo[] GetLineInfo(IVsTextLines buffer, int line, IVsTextColorState colorState);

        public virtual void Suspend()
        {
            suspended++;
        }

        public virtual void Resume()
        {
            suspended--;
            Debug.Assert(suspended >= 0);
        }

    }

#if COLORABLE_ITEM
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ColorableItem : IVsColorableItem, IVsHiColorItem, IVsMergeableUIItem {
        string name, displayName;
        COLORINDEX foreColor, backColor;
        Color hiForeColor, hiBackColor;
        FONTFLAGS fontFlags;

        internal ColorableItem(string name, string displayName, COLORINDEX foreColor, COLORINDEX backColor, Color hiForeColor, Color hiBackColor, FONTFLAGS fontFlags) {
            this.name = name;
            this.displayName = displayName;
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.fontFlags = fontFlags;
            this.hiForeColor = hiForeColor;
            this.hiBackColor = hiBackColor;
        }

        public virtual int GetDefaultColors(COLORINDEX[] foreColor, COLORINDEX[] backColor) {
            if (foreColor != null) foreColor[0] = this.foreColor;
            if (backColor != null) backColor[0] = this.backColor;
            return NativeMethods.S_OK;
        }
        public virtual int GetDefaultFontFlags(out uint fontFlags) {
            fontFlags = (uint)this.fontFlags;
            return NativeMethods.S_OK;
        }
        public virtual int GetDisplayName(out string name) {
            name = this.displayName;
            return NativeMethods.S_OK;
        }

        public virtual int GetColorData(int cdElement, out uint crColor) 
        {
            crColor = 0;

            switch (cdElement) 
            {
                case (int)__tagVSCOLORDATA.CD_FOREGROUND: 
                    {
                        if (!this.hiForeColor.IsEmpty) 
                        {
                            crColor = ColorToRgb(this.hiForeColor);
                            return NativeMethods.S_OK;
                        }
                        break;
                    }
                case (int)__tagVSCOLORDATA.CD_BACKGROUND: 
                    {
                        if (!this.hiBackColor.IsEmpty) 
                        {
                            crColor = ColorToRgb(this.hiBackColor);
                            return NativeMethods.S_OK;
                        }
                        break;
                    }
                default:
                    return NativeMethods.E_FAIL;
            }

            return NativeMethods.E_FAIL;
        }

        private uint ColorToRgb(Color color) 
        {
            byte red = (byte)color.R;
            short green = (short)(byte)color.G;
            int blue = (byte)color.B;

            return (uint)(red | (green << 8) | (blue << 16));
        }


        public virtual int GetCanonicalName(out string name) {
            name = this.name;
            return NativeMethods.S_OK;
        }

        public virtual int GetDescription(out string desc) {
            // The reason this is not implemented is because the core text editor
            // doesn't use it.
            desc = null;
            return NativeMethods.E_NOTIMPL;
        }

        public virtual int GetMergingPriority(out int priority) {
            priority = -1;
            return NativeMethods.E_NOTIMPL;
        }

    }
#endif

}
