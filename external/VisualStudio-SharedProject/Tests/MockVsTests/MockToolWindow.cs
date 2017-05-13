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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockToolWindow : IVsWindowFrame {
        private readonly object _docView;

        public MockToolWindow(object docView) {
            _docView = docView;
        }

        public int CloseFrame(uint grfSaveOptions) {
            throw new NotImplementedException();
        }

        public int GetFramePos(VSSETFRAMEPOS[] pdwSFP, out Guid pguidRelativeTo, out int px, out int py, out int pcx, out int pcy) {
            throw new NotImplementedException();
        }

        public int GetGuidProperty(int propid, out Guid pguid) {
            throw new NotImplementedException();
        }

        public int GetProperty(int propid, out object pvar) {
            if (propid == (int)__VSFPROPID.VSFPROPID_DocView) {
                pvar = _docView;
                return VSConstants.S_OK;
            }

            throw new NotImplementedException();
        }

        public int Hide() {
            throw new NotImplementedException();
        }

        public int IsOnScreen(out int pfOnScreen) {
            throw new NotImplementedException();
        }

        public int IsVisible() {
            throw new NotImplementedException();
        }

        public int QueryViewInterface(ref Guid riid, out IntPtr ppv) {
            throw new NotImplementedException();
        }

        public int SetFramePos(VSSETFRAMEPOS dwSFP, ref Guid rguidRelativeTo, int x, int y, int cx, int cy) {
            throw new NotImplementedException();
        }

        public int SetGuidProperty(int propid, ref Guid rguid) {
            throw new NotImplementedException();
        }

        public int SetProperty(int propid, object var) {
            throw new NotImplementedException();
        }

        public int Show() {
            throw new NotImplementedException();
        }

        public int ShowNoActivate() {
            throw new NotImplementedException();
        }
    }
}
