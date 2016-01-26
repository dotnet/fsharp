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
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudioTools.Project {
    internal static class UnsafeNativeMethods {
        [DllImport(ExternDll.Kernel32, EntryPoint = "GlobalLock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GlobalLock(IntPtr h);

        [DllImport(ExternDll.Kernel32, EntryPoint = "GlobalUnlock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GlobalUnLock(IntPtr h);

        [DllImport(ExternDll.Kernel32, EntryPoint = "GlobalSize", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GlobalSize(IntPtr h);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int OleSetClipboard(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int OleGetClipboard(out Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int OleFlushClipboard();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int OpenClipboard(IntPtr newOwner);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int EmptyClipboard();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int CloseClipboard();

        [DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
        internal static extern int ImageList_GetImageCount(HandleRef himl);

        [DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
        internal static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

        [DllImport(ExternDll.Shell32, EntryPoint = "DragQueryFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern uint RegisterClipboardFormat(string format);
    }
}

