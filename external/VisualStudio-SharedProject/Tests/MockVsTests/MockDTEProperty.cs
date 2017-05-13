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
using EnvDTE;

namespace Microsoft.VisualStudioTools.MockVsTests {
    internal class MockDTEProperty : Property {
        private object _value;

        public MockDTEProperty(object value) {
            _value = value;
        }

        public object Application {
            get {
                throw new NotImplementedException();
            }
        }

        public Properties Collection {
            get {
                throw new NotImplementedException();
            }
        }

        public DTE DTE {
            get {
                throw new NotImplementedException();
            }
        }

        public string Name {
            get {
                throw new NotImplementedException();
            }
        }

        public short NumIndices {
            get {
                throw new NotImplementedException();
            }
        }

        public object Object {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public Properties Parent {
            get {
                throw new NotImplementedException();
            }
        }

        public object Value {
            get {
                return _value;
            }

            set {
                _value = value;
            }
        }

        public object get_IndexedValue(object Index1, object Index2, object Index3, object Index4) {
            throw new NotImplementedException();
        }

        public void let_Value(object lppvReturn) {
            throw new NotImplementedException();
        }

        public void set_IndexedValue(object Index1, object Index2, object Index3, object Index4, object Val) {
            throw new NotImplementedException();
        }
    }
}