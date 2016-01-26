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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using TestUtilities.Mocks;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(MockVsEditorAdaptersFactoryService))]
    [Export(typeof(IVsEditorAdaptersFactoryService))]
    class MockVsEditorAdaptersFactoryService : IVsEditorAdaptersFactoryService {
        private readonly IServiceProvider _serviceProvider;
        
        [ImportingConstructor]
        public MockVsEditorAdaptersFactoryService([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }
        
        public VisualStudio.TextManager.Interop.IVsCodeWindow CreateVsCodeWindowAdapter(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider) {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextBuffer CreateVsTextBufferAdapter(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, VisualStudio.Utilities.IContentType contentType) {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextBuffer CreateVsTextBufferAdapter(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider) {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextBuffer CreateVsTextBufferAdapterForSecondaryBuffer(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, VisualStudio.Text.ITextBuffer secondaryBuffer) {
            throw new NotImplementedException();
        }

        public Microsoft.VisualStudio.TextManager.Interop.IVsTextBufferCoordinator CreateVsTextBufferCoordinatorAdapter() {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextView CreateVsTextViewAdapter(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, VisualStudio.Text.Editor.ITextViewRoleSet roles) {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextView CreateVsTextViewAdapter(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider) {
            throw new NotImplementedException();
        }

        public VisualStudio.TextManager.Interop.IVsTextBuffer GetBufferAdapter(VisualStudio.Text.ITextBuffer textBuffer) {
            MockVsTextLines textLines;
            if (!textBuffer.Properties.TryGetProperty<MockVsTextLines>(typeof(MockVsTextLines), out textLines)) {
                textBuffer.Properties[typeof(MockVsTextLines)] = textLines = new MockVsTextLines(_serviceProvider, (MockTextBuffer)textBuffer);
            }
            return textLines;
        }

        public VisualStudio.Text.ITextBuffer GetDataBuffer(VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter) {
            throw new NotImplementedException();
        }

        public VisualStudio.Text.ITextBuffer GetDocumentBuffer(VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter) {
            throw new NotImplementedException();
        }

        public IVsTextView GetViewAdapter(VisualStudio.Text.Editor.ITextView textView) {
            return textView.Properties.GetProperty<MockVsTextView>(typeof(MockVsTextView));
        }

        public IWpfTextView GetWpfTextView(IVsTextView viewAdapter) {
            return ((MockVsTextView)viewAdapter).View;
        }

        public IWpfTextViewHost GetWpfTextViewHost(IVsTextView viewAdapter) {
            throw new NotImplementedException();
        }

        public void SetDataBuffer(VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter, VisualStudio.Text.ITextBuffer dataBuffer) {
            throw new NotImplementedException();
        }
    }
}
