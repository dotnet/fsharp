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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.VisualStudioTools.Project {
    internal static class NativeMethods {
        // IIDS
        public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

        public const int ERROR_FILE_NOT_FOUND = 2;

        public const int
        CLSCTX_INPROC_SERVER = 0x1;

        public const int
        S_FALSE = 0x00000001,
        S_OK = 0x00000000,

        IDOK = 1,
        IDCANCEL = 2,
        IDABORT = 3,
        IDRETRY = 4,
        IDIGNORE = 5,
        IDYES = 6,
        IDNO = 7,
        IDCLOSE = 8,
        IDHELP = 9,
        IDTRYAGAIN = 10,
        IDCONTINUE = 11,

        OLECMDERR_E_NOTSUPPORTED = unchecked((int)0x80040100),
        OLECMDERR_E_UNKNOWNGROUP = unchecked((int)0x80040104),

        UNDO_E_CLIENTABORT = unchecked((int)0x80044001),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_INVALIDARG = unchecked((int)0x80070057),
        E_FAIL = unchecked((int)0x80004005),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_NOTIMPL = unchecked((int)0x80004001),
        E_UNEXPECTED = unchecked((int)0x8000FFFF),
        E_HANDLE = unchecked((int)0x80070006),
        E_ABORT = unchecked((int)0x80004004),
        E_ACCESSDENIED = unchecked((int)0x80070005),
        E_PENDING = unchecked((int)0x8000000A);

        public const int
        OLECLOSE_SAVEIFDIRTY = 0,
        OLECLOSE_NOSAVE = 1,
        OLECLOSE_PROMPTSAVE = 2;

        public const int
        OLEIVERB_PRIMARY = 0,
        OLEIVERB_SHOW = -1,
        OLEIVERB_OPEN = -2,
        OLEIVERB_HIDE = -3,
        OLEIVERB_UIACTIVATE = -4,
        OLEIVERB_INPLACEACTIVATE = -5,
        OLEIVERB_DISCARDUNDOSTATE = -6,
        OLEIVERB_PROPERTIES = -7;

        public const int
        OFN_READONLY = unchecked((int)0x00000001),
        OFN_OVERWRITEPROMPT = unchecked((int)0x00000002),
        OFN_HIDEREADONLY = unchecked((int)0x00000004),
        OFN_NOCHANGEDIR = unchecked((int)0x00000008),
        OFN_SHOWHELP = unchecked((int)0x00000010),
        OFN_ENABLEHOOK = unchecked((int)0x00000020),
        OFN_ENABLETEMPLATE = unchecked((int)0x00000040),
        OFN_ENABLETEMPLATEHANDLE = unchecked((int)0x00000080),
        OFN_NOVALIDATE = unchecked((int)0x00000100),
        OFN_ALLOWMULTISELECT = unchecked((int)0x00000200),
        OFN_EXTENSIONDIFFERENT = unchecked((int)0x00000400),
        OFN_PATHMUSTEXIST = unchecked((int)0x00000800),
        OFN_FILEMUSTEXIST = unchecked((int)0x00001000),
        OFN_CREATEPROMPT = unchecked((int)0x00002000),
        OFN_SHAREAWARE = unchecked((int)0x00004000),
        OFN_NOREADONLYRETURN = unchecked((int)0x00008000),
        OFN_NOTESTFILECREATE = unchecked((int)0x00010000),
        OFN_NONETWORKBUTTON = unchecked((int)0x00020000),
        OFN_NOLONGNAMES = unchecked((int)0x00040000),
        OFN_EXPLORER = unchecked((int)0x00080000),
        OFN_NODEREFERENCELINKS = unchecked((int)0x00100000),
        OFN_LONGNAMES = unchecked((int)0x00200000),
        OFN_ENABLEINCLUDENOTIFY = unchecked((int)0x00400000),
        OFN_ENABLESIZING = unchecked((int)0x00800000),
        OFN_USESHELLITEM = unchecked((int)0x01000000),
        OFN_DONTADDTORECENT = unchecked((int)0x02000000),
        OFN_FORCESHOWHIDDEN = unchecked((int)0x10000000);

        // for READONLYSTATUS
        public const int
        ROSTATUS_NotReadOnly = 0x0,
        ROSTATUS_ReadOnly = 0x1,
        ROSTATUS_Unknown = unchecked((int)0xFFFFFFFF);

        public const int
        IEI_DoNotLoadDocData = 0x10000000;

        public const int
        CB_SETDROPPEDWIDTH = 0x0160,

        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),

        DWL_MSGRESULT = 0,

        SW_SHOWNORMAL = 1,

        HTMENU = 5,

        WS_POPUP = unchecked((int)0x80000000),
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_TABSTOP = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        LVM_SETEXTENDEDLISTVIEWSTYLE = (0x1000 + 54),

        LVS_EX_LABELTIP = 0x00004000,

        // winuser.h
            WH_JOURNALPLAYBACK = 1,
        WH_GETMESSAGE = 3,
        WH_MOUSE = 7,
        WSF_VISIBLE = 0x0001,
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DELETEITEM = 0x002D,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WA_INACTIVE = 0,
        WA_ACTIVE = 1,
        WA_CLICKACTIVE = 2,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_NCXBUTTONDOWN = 0x00AB,
        WM_NCXBUTTONUP = 0x00AC,
        WM_NCXBUTTONDBLCLK = 0x00AD,
        WM_KEYFIRST = 0x0100,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_CTLCOLOR = 0x0019,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_CHANGEUISTATE = 0x0127,
        WM_UPDATEUISTATE = 0x0128,
        WM_QUERYUISTATE = 0x0129,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSELAST = 0x020A,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_POWERBROADCAST = 0x0218,
        WM_DEVICECHANGE = 0x0219,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = unchecked((int)0x8000),
        WM_USER = 0x0400,
        WM_REFLECT =
        WM_USER + 0x1C00,
        WS_OVERLAPPED = 0x00000000,
        WPF_SETMINPOSITION = 0x0001,
        WM_CHOOSEFONT_GETLOGFONT = (0x0400 + 1),

        WHEEL_DELTA = 120,
        DWLP_MSGRESULT = 0,
        PSNRET_NOERROR = 0,
        PSNRET_INVALID = 1,
        PSNRET_INVALID_NOCHANGEPAGE = 2,
        EM_SETCUEBANNER = 0x1501;

        public const int
        PSN_APPLY = ((0 - 200) - 2),
        PSN_KILLACTIVE = ((0 - 200) - 1),
        PSN_RESET = ((0 - 200) - 3),
        PSN_SETACTIVE = ((0 - 200) - 0);

        public const int
        GMEM_MOVEABLE = 0x0002,
        GMEM_ZEROINIT = 0x0040,
        GMEM_DDESHARE = 0x2000;

        public const int
        SWP_NOACTIVATE = 0x0010,
        SWP_NOZORDER = 0x0004,
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_FRAMECHANGED = 0x0020;

        public const int
        TVM_SETINSERTMARK = (0x1100 + 26),
        TVM_GETEDITCONTROL = (0x1100 + 15);

        public const int
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        public const int
            PSP_DEFAULT = 0x00000000,
            PSP_DLGINDIRECT = 0x00000001,
            PSP_USEHICON = 0x00000002,
            PSP_USEICONID = 0x00000004,
            PSP_USETITLE = 0x00000008,
            PSP_RTLREADING = 0x00000010,
            PSP_HASHELP = 0x00000020,
            PSP_USEREFPARENT = 0x00000040,
            PSP_USECALLBACK = 0x00000080,
            PSP_PREMATURE = 0x00000400,
            PSP_HIDEHEADER = 0x00000800,
            PSP_USEHEADERTITLE = 0x00001000,
            PSP_USEHEADERSUBTITLE = 0x00002000;

        public const int
            PSH_DEFAULT = 0x00000000,
            PSH_PROPTITLE = 0x00000001,
            PSH_USEHICON = 0x00000002,
            PSH_USEICONID = 0x00000004,
            PSH_PROPSHEETPAGE = 0x00000008,
            PSH_WIZARDHASFINISH = 0x00000010,
            PSH_WIZARD = 0x00000020,
            PSH_USEPSTARTPAGE = 0x00000040,
            PSH_NOAPPLYNOW = 0x00000080,
            PSH_USECALLBACK = 0x00000100,
            PSH_HASHELP = 0x00000200,
            PSH_MODELESS = 0x00000400,
            PSH_RTLREADING = 0x00000800,
            PSH_WIZARDCONTEXTHELP = 0x00001000,
            PSH_WATERMARK = 0x00008000,
            PSH_USEHBMWATERMARK = 0x00010000,  // user pass in a hbmWatermark instead of pszbmWatermark
            PSH_USEHPLWATERMARK = 0x00020000,  //
            PSH_STRETCHWATERMARK = 0x00040000,  // stretchwatermark also applies for the header
            PSH_HEADER = 0x00080000,
            PSH_USEHBMHEADER = 0x00100000,
            PSH_USEPAGELANG = 0x00200000,  // use frame dialog template matched to page
            PSH_WIZARD_LITE = 0x00400000,
            PSH_NOCONTEXTHELP = 0x02000000;

        public const int
            PSBTN_BACK = 0,
            PSBTN_NEXT = 1,
            PSBTN_FINISH = 2,
            PSBTN_OK = 3,
            PSBTN_APPLYNOW = 4,
            PSBTN_CANCEL = 5,
            PSBTN_HELP = 6,
            PSBTN_MAX = 6;

        public const int
            TRANSPARENT = 1,
            OPAQUE = 2,
            FW_BOLD = 700;

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR {
            public IntPtr hwndFrom;
            public int idFrom;
            public int code;
        }

        /// <devdoc>
        /// Helper class for setting the text parameters to OLECMDTEXT structures.
        /// </devdoc>
        public static class OLECMDTEXT {
            public static void SetText(IntPtr pCmdTextInt, string text) {
                Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
                char[] menuText = text.ToCharArray();

                // Get the offset to the rgsz param.  This is where we will stuff our text
                //
                IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
                IntPtr offsetToCwActual = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "cwActual");

                // The max chars we copy is our string, or one less than the buffer size,
                // since we need a null at the end.
                //
                int maxChars = Math.Min((int)pCmdText.cwBuf - 1, menuText.Length);

                Marshal.Copy(menuText, 0, (IntPtr)((long)pCmdTextInt + (long)offset), maxChars);

                // append a null character
                Marshal.WriteInt16((IntPtr)((long)pCmdTextInt + (long)offset + maxChars * 2), 0);

                // write out the length
                // +1 for the null char
                Marshal.WriteInt32((IntPtr)((long)pCmdTextInt + (long)offsetToCwActual), maxChars + 1);
            }

            /// <summary>
            /// Gets the flags of the OLECMDTEXT structure
            /// </summary>
            /// <param name="pCmdTextInt">The structure to read.</param>
            /// <returns>The value of the flags.</returns>
            public static OLECMDTEXTF GetFlags(IntPtr pCmdTextInt) {
                Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

                if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_NAME) != 0)
                    return OLECMDTEXTF.OLECMDTEXTF_NAME;

                if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_STATUS) != 0)
                    return OLECMDTEXTF.OLECMDTEXTF_STATUS;

                return OLECMDTEXTF.OLECMDTEXTF_NONE;
            }


            /// <summary>
            /// Flags for the OLE command text
            /// </summary>
            public enum OLECMDTEXTF {
                /// <summary>No flag</summary>
                OLECMDTEXTF_NONE = 0,
                /// <summary>The name of the command is required.</summary>
                OLECMDTEXTF_NAME = 1,
                /// <summary>A description of the status is required.</summary>
                OLECMDTEXTF_STATUS = 2
            }
        }

        /// <devdoc>
        /// OLECMDF enums for IOleCommandTarget
        /// </devdoc>
        public enum tagOLECMDF {
            OLECMDF_SUPPORTED = 1,
            OLECMDF_ENABLED = 2,
            OLECMDF_LATCHED = 4,
            OLECMDF_NINCHED = 8,
            OLECMDF_INVISIBLE = 16
        }

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetClientRect(IntPtr hWnd, out User32RECT lpRect);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessageW(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void SetErrorDescription(string description, params object[] args) {
            ICreateErrorInfo errInfo;
            ErrorHandler.ThrowOnFailure(CreateErrorInfo(out errInfo));

            errInfo.SetDescription(String.Format(description, args));
            var guidNull = Guid.Empty;
            errInfo.SetGUID(ref guidNull);
            errInfo.SetHelpFile(null);
            errInfo.SetHelpContext(0);
            errInfo.SetSource("");
            IErrorInfo errorInfo = errInfo as IErrorInfo;
            SetErrorInfo(0, errorInfo);
        }

        [DllImport("oleaut32")]
        static extern int CreateErrorInfo(out ICreateErrorInfo errInfo);

        [DllImport("oleaut32")]
        static extern int SetErrorInfo(uint dwReserved, IErrorInfo perrinfo);

        [ComImport(), Guid("9BDA66AE-CA28-4e22-AA27-8A7218A0E3FA"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEventHandler {

            // converts the underlying codefunction into an event handler for the given event
            // if the given event is NULL, then the function will handle no events
            [PreserveSig]
            int AddHandler(string bstrEventName);

            [PreserveSig]
            int RemoveHandler(string bstrEventName);

            IVsEnumBSTR GetHandledEvents();

            bool HandlesEvent(string bstrEventName);
        }

        [ComImport(), Guid("A55CCBCC-7031-432d-B30A-A68DE7BDAD75"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IParameterKind {

            void SetParameterPassingMode(PARAMETER_PASSING_MODE ParamPassingMode);
            void SetParameterArrayDimensions(int uDimensions);
            int GetParameterArrayCount();
            int GetParameterArrayDimensions(int uIndex);
            int GetParameterPassingMode();
        }

        public enum PARAMETER_PASSING_MODE {
            cmParameterTypeIn = 1,
            cmParameterTypeOut = 2,
            cmParameterTypeInOut = 3
        }

        [
        ComImport, ComVisible(true), Guid("3E596484-D2E4-461a-A876-254C4F097EBB"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMethodXML {
            // Generate XML describing the contents of this function's body.
            void GetXML(ref string pbstrXML);

            // Parse the incoming XML with respect to the CodeModel XML schema and
            // use the result to regenerate the body of the function.
            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="IMethodXML.SetXML"]/*' />
            [PreserveSig]
            int SetXML(string pszXML);

            // This is really a textpoint
            [PreserveSig]
            int GetBodyPoint([MarshalAs(UnmanagedType.Interface)]out object bodyPoint);
        }

        [ComImport(), Guid("EA1A87AD-7BC5-4349-B3BE-CADC301F17A3"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVBFileCodeModelEvents {

            [PreserveSig]
            int StartEdit();

            [PreserveSig]
            int EndEdit();
        }

        ///--------------------------------------------------------------------------
        /// ICodeClassBase:
        ///--------------------------------------------------------------------------
        [GuidAttribute("23BBD58A-7C59-449b-A93C-43E59EFC080C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport()]
        public interface ICodeClassBase {
            [PreserveSig()]
            int GetBaseName(out string pBaseName);
        }

        public const ushort CF_HDROP = 15; // winuser.h
        public const uint MK_CONTROL = 0x0008; //winuser.h
        public const uint MK_SHIFT = 0x0004;
        public const int MAX_PATH = 260; // windef.h	
        public const int MAX_FOLDER_PATH = MAX_PATH - 12;   // folders need to allow 8.3 filenames, so MAX_PATH - 12

        /// <summary>
        /// Specifies options for a bitmap image associated with a task item.
        /// </summary>
        public enum VSTASKBITMAP {
            BMP_COMPILE = -1,
            BMP_SQUIGGLE = -2,
            BMP_COMMENT = -3,
            BMP_SHORTCUT = -4,
            BMP_USER = -5
        };

        public const int ILD_NORMAL = 0x0000,
            ILD_TRANSPARENT = 0x0001,
            ILD_MASK = 0x0010,
            ILD_ROP = 0x0040;

        /// <summary>
        /// Defines the values that are not supported by the System.Environment.SpecialFolder enumeration
        /// </summary>
        [ComVisible(true)]
        public enum ExtendedSpecialFolder {
            /// <summary>
            /// Identical to CSIDL_COMMON_STARTUP
            /// </summary>
            CommonStartup = 0x0018,

            /// <summary>
            /// Identical to CSIDL_WINDOWS 
            /// </summary>
            Windows = 0x0024,
        }


        // APIS

        /// <summary>
        /// Changes the parent window of the specified child window.
        /// </summary>
        /// <param name="hWnd">Handle to the child window.</param>
        /// <param name="hWndParent">Handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window.</param>
        /// <returns>A handle to the previous parent window indicates success. NULL indicates failure.</returns>
        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "IsDialogMessageA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool IsDialogMessageA(IntPtr hDlg, ref MSG msg);

        [DllImport("kernel32", EntryPoint = "GetBinaryTypeW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        private static extern bool _GetBinaryType(string lpApplicationName, out GetBinaryTypeResult lpBinaryType);

        private enum GetBinaryTypeResult : uint {
            SCS_32BIT_BINARY = 0,
            SCS_DOS_BINARY = 1,
            SCS_WOW_BINARY = 2,
            SCS_PIF_BINARY = 3,
            SCS_POSIX_BINARY = 4,
            SCS_OS216_BINARY = 5,
            SCS_64BIT_BINARY = 6
        }

        public static ProcessorArchitecture GetBinaryType(string path) {
            GetBinaryTypeResult result;

            if (_GetBinaryType(path, out result)) {
                switch (result) {
                    case GetBinaryTypeResult.SCS_32BIT_BINARY:
                        return ProcessorArchitecture.X86;
                    case GetBinaryTypeResult.SCS_64BIT_BINARY:
                        return ProcessorArchitecture.Amd64;
                    case GetBinaryTypeResult.SCS_DOS_BINARY:
                    case GetBinaryTypeResult.SCS_WOW_BINARY:
                    case GetBinaryTypeResult.SCS_PIF_BINARY:
                    case GetBinaryTypeResult.SCS_POSIX_BINARY:
                    case GetBinaryTypeResult.SCS_OS216_BINARY:
                    default:
                        break;
                }
            }

            return ProcessorArchitecture.None;
        }


        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
           int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        [DllImport("ADVAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LogonUser(
            [In] string lpszUsername,
            [In] string lpszDomain,
            [In] string lpszPassword,
            [In] LogonType dwLogonType,
            [In] LogonProvider dwLogonProvider,
            [In, Out] ref IntPtr hToken
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetFinalPathNameByHandle(
            SafeHandle hFile,
            [Out]StringBuilder lpszFilePath,
            uint cchFilePath,
            uint dwFlags
        );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(
            string lpFileName,
            FileDesiredAccess dwDesiredAccess,
            FileShareFlags dwShareMode,
            IntPtr lpSecurityAttributes,
            FileCreationDisposition dwCreationDisposition,
            FileFlagsAndAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [Flags]
        public enum FileDesiredAccess : uint {
            FILE_LIST_DIRECTORY = 1
        }

        [Flags]
        public enum FileShareFlags : uint {
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_DELETE = 0x00000004
        }

        [Flags]
        public enum FileCreationDisposition : uint {
            OPEN_EXISTING = 3
        }

        [Flags]
        public enum FileFlagsAndAttributes : uint {
            FILE_FLAG_BACKUP_SEMANTICS = 0x02000000
        }

        public static IntPtr INVALID_FILE_HANDLE = new IntPtr(-1);
        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public enum LogonType {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK,
            LOGON32_LOGON_BATCH,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT,
            LOGON32_LOGON_NEW_CREDENTIALS
        }

        public enum LogonProvider {
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT35,
            LOGON32_PROVIDER_WINNT40,
            LOGON32_PROVIDER_WINNT50
        }

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern MsiInstallState MsiGetComponentPath(string szProduct, string szComponent, [Out]StringBuilder lpPathBuf, ref uint pcchBuf);

        /// <summary>
        /// Buffer for lpProductBuf must be 39 characters long.
        /// </summary>
        /// <param name="szComponent"></param>
        /// <param name="lpProductBuf"></param>
        /// <returns></returns>
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiGetProductCode(string szComponent, [Out]StringBuilder lpProductBuf);


        internal enum MsiInstallState {
            NotUsed = -7,  // component disabled
            BadConfig = -6,  // configuration data corrupt
            Incomplete = -5,  // installation suspended or in progress
            SourceAbsent = -4,  // run from source, source is unavailable
            MoreData = -3,  // return buffer overflow
            InvalidArg = -2,  // invalid function argument
            Unknown = -1,  // unrecognized product or feature
            Broken = 0,  // broken
            Advertised = 1,  // advertised feature
            Removed = 1,  // component being removed (action state, not settable)
            Absent = 2,  // uninstalled (or action state absent but clients remain)
            Local = 3,  // installed on local drive
            Source = 4,  // run from source, CD or net
            Default = 5  // use default, local or source
        }

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("mpr", CharSet = CharSet.Unicode)]
        public static extern uint WNetAddConnection3(IntPtr handle, ref _NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        public const int CONNECT_INTERACTIVE = 0x08;
        public const int CONNECT_PROMPT = 0x10;
        public const int RESOURCETYPE_DISK = 1;

        public struct _NETRESOURCE {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        [DllImport(ExternDll.Kernel32, EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetFinalPathNameByHandle(SafeFileHandle handle, [In, Out] StringBuilder path, int bufLen, int flags);

        [DllImport(ExternDll.Kernel32, EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr SecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// Given a directory, actual or symbolic, return the actual directory path.
        /// </summary>
        /// <param name="symlink">DirectoryInfo object for the suspected symlink.</param>
        /// <returns>A string of the actual path.</returns>
        internal static string GetAbsolutePathToDirectory(string symlink) {
            const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            const int DEVICE_QUERY_ACCESS = 0;

            using (SafeFileHandle directoryHandle = CreateFile(
                symlink,
                DEVICE_QUERY_ACCESS,
                FileShare.Write,
                System.IntPtr.Zero,
                FileMode.Open,
                FILE_FLAG_BACKUP_SEMANTICS,
                System.IntPtr.Zero)) {
                if (directoryHandle.IsInvalid) {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                StringBuilder path = new StringBuilder(512);
                int pathSize = GetFinalPathNameByHandle(directoryHandle, path, path.Capacity, 0);
                if (pathSize < 0) {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                // UNC Paths will start with \\?\.  Remove this if present as this isn't really expected on a path.
                var pathString = path.ToString();
                return pathString.StartsWith(@"\\?\") ? pathString.Substring(4) : pathString;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool RemoveDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool MoveFile(String src, String dst);
    }

    internal class CredUI {
        private const string advapi32Dll = "advapi32.dll";
        private const string credUIDll = "credui.dll";

        public const int
        ERROR_INVALID_FLAGS = 1004,  // Invalid flags.
        ERROR_NOT_FOUND = 1168,  // Element not found.
        ERROR_NO_SUCH_LOGON_SESSION = 1312,  // A specified logon session does not exist. It may already have been terminated.
        ERROR_LOGON_FAILURE = 1326;  // Logon failure: unknown user name or bad password.

        [Flags]
        public enum CREDUI_FLAGS : uint {
            INCORRECT_PASSWORD = 0x1,
            DO_NOT_PERSIST = 0x2,
            REQUEST_ADMINISTRATOR = 0x4,
            EXCLUDE_CERTIFICATES = 0x8,
            REQUIRE_CERTIFICATE = 0x10,
            SHOW_SAVE_CHECK_BOX = 0x40,
            ALWAYS_SHOW_UI = 0x80,
            REQUIRE_SMARTCARD = 0x100,
            PASSWORD_ONLY_OK = 0x200,
            VALIDATE_USERNAME = 0x400,
            COMPLETE_USERNAME = 0x800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CREDUI_INFO {
            public int cbSize;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr hwndParentCERParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCaptionText;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr hbmBannerCERHandle;
        }

        public enum CredUIReturnCodes : uint {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004,
        }

        // Copied from wincred.h
        public const uint
            // Values of the Credential Type field.
        CRED_TYPE_GENERIC = 1,
        CRED_TYPE_DOMAIN_PASSWORD = 2,
        CRED_TYPE_DOMAIN_CERTIFICATE = 3,
        CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4,
        CRED_TYPE_MAXIMUM = 5,                           // Maximum supported cred type
        CRED_TYPE_MAXIMUM_EX = (CRED_TYPE_MAXIMUM + 1000),    // Allow new applications to run on old OSes

        // String limits
        CRED_MAX_CREDENTIAL_BLOB_SIZE = 512,         // Maximum size of the CredBlob field (in bytes)
        CRED_MAX_STRING_LENGTH = 256,         // Maximum length of the various credential string fields (in characters)
        CRED_MAX_USERNAME_LENGTH = (256 + 1 + 256), // Maximum length of the UserName field.  The worst case is <User>@<DnsDomain>
        CRED_MAX_GENERIC_TARGET_NAME_LENGTH = 32767,       // Maximum length of the TargetName field for CRED_TYPE_GENERIC (in characters)
        CRED_MAX_DOMAIN_TARGET_NAME_LENGTH = (256 + 1 + 80),  // Maximum length of the TargetName field for CRED_TYPE_DOMAIN_* (in characters). Largest one is <DfsRoot>\<DfsShare>
        CRED_MAX_VALUE_SIZE = 256,         // Maximum size of the Credential Attribute Value field (in bytes)
        CRED_MAX_ATTRIBUTES = 64,          // Maximum number of attributes per credential
        CREDUI_MAX_MESSAGE_LENGTH = 32767,
        CREDUI_MAX_CAPTION_LENGTH = 128,
        CREDUI_MAX_GENERIC_TARGET_LENGTH = CRED_MAX_GENERIC_TARGET_NAME_LENGTH,
        CREDUI_MAX_DOMAIN_TARGET_LENGTH = CRED_MAX_DOMAIN_TARGET_NAME_LENGTH,
        CREDUI_MAX_USERNAME_LENGTH = CRED_MAX_USERNAME_LENGTH,
        CREDUI_MAX_PASSWORD_LENGTH = (CRED_MAX_CREDENTIAL_BLOB_SIZE / 2);

        internal enum CRED_PERSIST : uint {
            NONE = 0,
            SESSION = 1,
            LOCAL_MACHINE = 2,
            ENTERPRISE = 3,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NativeCredential {
            public uint flags;
            public uint type;
            public string targetName;
            public string comment;
            public int lastWritten_lowDateTime;
            public int lastWritten_highDateTime;
            public uint credentialBlobSize;
            public IntPtr credentialBlob;
            public uint persist;
            public uint attributeCount;
            public IntPtr attributes;
            public string targetAlias;
            public string userName;
        };

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport(advapi32Dll, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CredReadW")]
        public static extern bool
        CredRead(
            [MarshalAs(UnmanagedType.LPWStr)]
            string targetName,
            [MarshalAs(UnmanagedType.U4)]
            uint type,
            [MarshalAs(UnmanagedType.U4)]
            uint flags,
            out IntPtr credential
            );

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport(advapi32Dll, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CredWriteW")]
        public static extern bool
        CredWrite(
            ref NativeCredential Credential,
            [MarshalAs(UnmanagedType.U4)]
            uint flags
            );

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport(advapi32Dll, SetLastError = true)]
        public static extern bool
        CredFree(
            IntPtr buffer
            );

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport(credUIDll, EntryPoint = "CredUIPromptForCredentialsW", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUIPromptForCredentials(
            CREDUI_INFO pUiInfo,  // Optional (one can pass null here)
            [MarshalAs(UnmanagedType.LPWStr)]
            string targetName,
            IntPtr Reserved,      // Must be 0 (IntPtr.Zero)
            int iError,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder pszUserName,
            [MarshalAs(UnmanagedType.U4)]
            uint ulUserNameMaxChars,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder pszPassword,
            [MarshalAs(UnmanagedType.U4)]
            uint ulPasswordMaxChars,
            ref int pfSave,
            CREDUI_FLAGS dwFlags);

        /// <returns>
        /// Win32 system errors:
        /// NO_ERROR
        /// ERROR_INVALID_ACCOUNT_NAME
        /// ERROR_INSUFFICIENT_BUFFER
        /// ERROR_INVALID_PARAMETER
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport(credUIDll, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CredUIParseUserNameW")]
        public static extern CredUIReturnCodes CredUIParseUserName(
            [MarshalAs(UnmanagedType.LPWStr)]
            string strUserName,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder strUser,
            [MarshalAs(UnmanagedType.U4)]
            uint iUserMaxChars,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder strDomain,
            [MarshalAs(UnmanagedType.U4)]
            uint iDomainMaxChars
            );
    }

    struct User32RECT {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width {
            get {
                return right - left;
            }
        }

        public int Height {
            get {
                return bottom - top;
            }
        }
    }

    [Guid("22F03340-547D-101B-8E65-08002B2BD119")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICreateErrorInfo {
        int SetGUID(
             ref Guid rguid
         );

        int SetSource(string szSource);

        int SetDescription(string szDescription);

        int SetHelpFile(string szHelpFile);

        int SetHelpContext(uint dwHelpContext);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WIN32_FIND_DATA {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }
}

