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
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(IQuickInfoBroker))]
    class MockQuickInfoBroker : IQuickInfoBroker {
        public IQuickInfoSession CreateQuickInfoSession(VisualStudio.Text.Editor.ITextView textView, VisualStudio.Text.ITrackingPoint triggerPoint, bool trackMouse) {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IQuickInfoSession> GetSessions(VisualStudio.Text.Editor.ITextView textView) {
            throw new NotImplementedException();
        }

        public bool IsQuickInfoActive(VisualStudio.Text.Editor.ITextView textView) {
            throw new NotImplementedException();
        }

        public IQuickInfoSession TriggerQuickInfo(VisualStudio.Text.Editor.ITextView textView, VisualStudio.Text.ITrackingPoint triggerPoint, bool trackMouse) {
            throw new NotImplementedException();
        }

        public IQuickInfoSession TriggerQuickInfo(VisualStudio.Text.Editor.ITextView textView) {
            throw new NotImplementedException();
        }
    }
}
