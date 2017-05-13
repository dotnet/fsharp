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
using Microsoft.PythonTools.Interpreter;
using Microsoft.TC.TestHostAdapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class DefaultInterpreterSetter : IDisposable {
        public readonly object OriginalInterpreter, OriginalVersion;
        private bool _isDisposed;

        public DefaultInterpreterSetter(IPythonInterpreterFactory factory) {
            var props = VsIdeTestHostContext.Dte.get_Properties("Python Tools", "Interpreters");
            Assert.IsNotNull(props);

            OriginalInterpreter = props.Item("DefaultInterpreter").Value;
            OriginalVersion = props.Item("DefaultInterpreterVersion").Value;

            props.Item("DefaultInterpreter").Value = factory.Id;
            props.Item("DefaultInterpreterVersion").Value = string.Format("{0}.{1}", factory.Configuration.Version.Major, factory.Configuration.Version.Minor);
        }

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;

                var props = VsIdeTestHostContext.Dte.get_Properties("Python Tools", "Interpreters");
                Assert.IsNotNull(props);

                props.Item("DefaultInterpreter").Value = OriginalInterpreter;
                props.Item("DefaultInterpreterVersion").Value = OriginalVersion;
            }
        }
    }
}
