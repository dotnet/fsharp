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
using System.Threading;
using TestUtilities;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockDialog {
        public readonly string Title;
        public readonly MockVs Vs;
        public int DialogResult = 0;
        private AutoResetEvent _dismiss = new AutoResetEvent(false);

        public MockDialog(MockVs vs, string title) {
            Title = title;
            Vs = vs;
        }

        public virtual void Type(string text) {
            switch (text) {
                case "\r":
                    Close((int)MessageBoxButton.Ok);
                    break;
                default:
                    throw new NotImplementedException("Unhandled dialog text: " + text);
            }
        }

        public virtual void Run() {
            Vs.RunMessageLoop(_dismiss);
        }

        public virtual void Close(int result) {
            DialogResult = result;
            _dismiss.Set();
        }
    }
}
