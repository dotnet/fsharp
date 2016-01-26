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
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities {
    public class SessionHolder<T> : IDisposable where T : IIntellisenseSession {
        public readonly T Session;
        private readonly IEditor _owner;

        public SessionHolder(T session, IEditor owner) {
            Assert.IsNotNull(session);
            Session = session;
            _owner = owner;
        }

        void IDisposable.Dispose() {
            if (!Session.IsDismissed) {
                _owner.Invoke(() => { Session.Dismiss(); });
            }
        }
    }

}
