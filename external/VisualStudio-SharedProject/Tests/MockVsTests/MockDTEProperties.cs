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
using System.Collections;
using System.Collections.Generic;
using EnvDTE;

namespace Microsoft.VisualStudioTools.MockVsTests {
    internal class MockDTEProperties : EnvDTE.Properties {
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        public MockDTEProperties() {
        }

        public object Application {
            get {
                throw new NotImplementedException();
            }
        }

        public int Count {
            get {
                return _properties.Count;
            }
        }

        public DTE DTE {
            get {
                throw new NotImplementedException();
            }
        }

        public object Parent {
            get {
                throw new NotImplementedException();
            }
        }

        public IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }

        public Property Item(object index) {
            return _properties[(string)index];
        }

        public void Add(string name, object value) {
            _properties.Add(name, new MockDTEProperty(value));
        }
    }
}