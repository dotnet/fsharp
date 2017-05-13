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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudioTools.Project;

namespace Microsoft.VisualStudioTools {

    class ClipboardService : IClipboardService {
        public void SetClipboard(IDataObject dataObject) {
            ErrorHandler.ThrowOnFailure(UnsafeNativeMethods.OleSetClipboard(dataObject));
        }

        public IDataObject GetClipboard() {
            IDataObject res;
            ErrorHandler.ThrowOnFailure(UnsafeNativeMethods.OleGetClipboard(out res));
            return res;
        }

        public void FlushClipboard() {
            ErrorHandler.ThrowOnFailure(UnsafeNativeMethods.OleFlushClipboard());
        }

        public bool OpenClipboard() {
            int res = UnsafeNativeMethods.OpenClipboard(IntPtr.Zero);
            ErrorHandler.ThrowOnFailure(res);
            return res == 1;
        }

        public void EmptyClipboard() {
            ErrorHandler.ThrowOnFailure(UnsafeNativeMethods.EmptyClipboard());
        }

        public void CloseClipboard() {
            ErrorHandler.ThrowOnFailure(UnsafeNativeMethods.CloseClipboard());
        }
    }
}