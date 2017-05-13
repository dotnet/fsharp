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
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockCodeWindow : IVsCodeWindow, Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITextView _view;

        public MockCodeWindow(IServiceProvider serviceProvider, ITextView view) {
            _serviceProvider = serviceProvider;
            _view = view;
        }

        public int Close() {
            throw new NotImplementedException();
        }

        public int GetBuffer(out IVsTextLines ppBuffer) {
            throw new NotImplementedException();
        }

        public int GetEditorCaption(READONLYSTATUS dwReadOnly, out string pbstrEditorCaption) {
            throw new NotImplementedException();
        }

        public int GetLastActiveView(out IVsTextView ppView) {
            throw new NotImplementedException();
        }

        public int GetPrimaryView(out IVsTextView ppView) {
            var compModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));
            var editorAdapters = compModel.GetService<IVsEditorAdaptersFactoryService>();
            ppView = editorAdapters.GetViewAdapter(_view);
            return VSConstants.S_OK;
        }

        public int GetSecondaryView(out IVsTextView ppView) {
            ppView = null;
            return VSConstants.E_FAIL;
        }

        public int GetViewClassID(out Guid pclsidView) {
            throw new NotImplementedException();
        }

        public int SetBaseEditorCaption(string[] pszBaseEditorCaption) {
            throw new NotImplementedException();
        }

        public int SetBuffer(IVsTextLines pBuffer) {
            throw new NotImplementedException();
        }

        public int SetViewClassID(ref Guid clsidView) {
            throw new NotImplementedException();
        }

        public void EnumConnectionPoints(out VisualStudio.OLE.Interop.IEnumConnectionPoints ppEnum) {
            throw new NotImplementedException();
        }

        public void FindConnectionPoint(ref Guid riid, out VisualStudio.OLE.Interop.IConnectionPoint ppCP) {
            ppCP = null;
        }
    }
}
