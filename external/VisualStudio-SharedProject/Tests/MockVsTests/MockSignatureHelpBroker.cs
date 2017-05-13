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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(ISignatureHelpBroker))]
    class MockSignatureHelpBroker : ISignatureHelpBroker {
        private readonly IEnumerable<Lazy<ISignatureHelpSourceProvider, IContentTypeMetadata>> _sigProviders;
        private readonly IIntellisenseSessionStackMapService _stackMap;

        [ImportingConstructor]
        public MockSignatureHelpBroker(IIntellisenseSessionStackMapService stackMap, [ImportMany]IEnumerable<Lazy<ISignatureHelpSourceProvider, IContentTypeMetadata>> sigProviders) {
            _stackMap = stackMap;
            _sigProviders = sigProviders;
        }

        public ISignatureHelpSession CreateSignatureHelpSession(VisualStudio.Text.Editor.ITextView textView, VisualStudio.Text.ITrackingPoint triggerPoint, bool trackCaret) {
            throw new NotImplementedException();
        }

        public void DismissAllSessions(VisualStudio.Text.Editor.ITextView textView) {
            foreach (var session in _stackMap.GetStackForTextView(textView).Sessions) {
                if (session is ISignatureHelpSession) {
                    session.Dismiss();
                }
            }
        }

        public ReadOnlyCollection<ISignatureHelpSession> GetSessions(VisualStudio.Text.Editor.ITextView textView) {
            List<ISignatureHelpSession> res = new List<ISignatureHelpSession>();
            foreach (var session in _stackMap.GetStackForTextView(textView).Sessions) {
                if (session is ISignatureHelpSession) {
                    res.Add(session as ISignatureHelpSession);
                }
            }
            return new ReadOnlyCollection<ISignatureHelpSession>(res);
        }

        public bool IsSignatureHelpActive(VisualStudio.Text.Editor.ITextView textView) {
            foreach (var session in _stackMap.GetStackForTextView(textView).Sessions) {
                if (session is ISignatureHelpSession) {
                    return true;
                }
            }
            return false;
        }

        public ISignatureHelpSession TriggerSignatureHelp(VisualStudio.Text.Editor.ITextView textView, VisualStudio.Text.ITrackingPoint triggerPoint, bool trackCaret) {
            throw new NotImplementedException();
        }

        public ISignatureHelpSession TriggerSignatureHelp(VisualStudio.Text.Editor.ITextView textView) {
            ObservableCollection<ISignature> sets = new ObservableCollection<ISignature>();
            var session = new MockSignatureHelpSession(
                textView,
                sets,
                textView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(
                    textView.Caret.Position.BufferPosition.Position,
                    PointTrackingMode.Negative
                )
            );

            foreach (var provider in _sigProviders) {
                foreach (var targetContentType in provider.Metadata.ContentTypes) {
                    if (textView.TextBuffer.ContentType.IsOfType(targetContentType)) {
                        var source = provider.Value.TryCreateSignatureHelpSource(textView.TextBuffer);
                        if (source != null) {
                            source.AugmentSignatureHelpSession(session, sets);
                        }
                    }
                }
            }

            if (session.Signatures.Count > 0 && !session.IsDismissed) {
                _stackMap.GetStackForTextView(textView).PushSession(session);
            }

            return session;
        }
    }
}
