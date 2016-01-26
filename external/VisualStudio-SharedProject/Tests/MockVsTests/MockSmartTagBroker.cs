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
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudioTools.MockVsTests {
#if DEV14_OR_LATER
#pragma warning disable 0618
#endif

    [Export(typeof(ISmartTagBroker))]
    public class MockSmartTagBroker : ISmartTagBroker {
        private readonly List<KeyValuePair<ITextView, ISmartTagSession>> _sessions = new List<KeyValuePair<ITextView, ISmartTagSession>>();

        public readonly List<ISmartTagSourceProvider> SourceProviders = new List<ISmartTagSourceProvider>();

        public ISmartTagSession CreateSmartTagSession(ITextView textView, SmartTagType type, ITrackingPoint triggerPoint, SmartTagState state) {
            var session = new MockSmartTagSession(this) {
                TextView = textView,
                Type = type,
                TriggerPoint = triggerPoint,
                State = state
            };
            lock (_sessions) {
                _sessions.Add(new KeyValuePair<ITextView, ISmartTagSession>(textView, session));
            }
            session.Dismissed += Session_Dismissed;
            return session;
        }

        private void Session_Dismissed(object sender, EventArgs e) {
            var session = sender as ISmartTagSession;
            if (session != null) {
                lock (_sessions) {
                    _sessions.RemoveAll(kv => kv.Value == session);
                }
            }
        }

        public ReadOnlyCollection<ISmartTagSession> GetSessions(ITextView textView) {
            lock (_sessions) {
                return new ReadOnlyCollection<ISmartTagSession>(
                    _sessions.Where(kv => kv.Key == textView).Select(kv => kv.Value).ToList()
                );
            }
        }

        public bool IsSmartTagActive(ITextView textView) {
            lock (_sessions) {
                return _sessions.Any(kv => kv.Key == textView);
            }
        }


    }
}
