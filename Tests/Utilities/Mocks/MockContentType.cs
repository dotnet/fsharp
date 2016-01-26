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

using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;

namespace TestUtilities.Mocks {
    public class MockContentType : IContentType {
        private readonly string _name;
        private readonly List<IContentType> _bases;

        public MockContentType(string name, IContentType[] bases) {
            _name = name;
            _bases = new List<IContentType>(bases);
        }

        IEnumerable<IContentType> IContentType.BaseTypes {
            get { return _bases; }
        }

        public List<IContentType> BaseTypes {
            get {
                return _bases;
            }
        }

        public bool IsOfType(string type) {
            if (type == _name) {
                return true;
            }

            foreach (var baseType in BaseTypes) {
                if (baseType.IsOfType(type)) {
                    return true;
                }
            }
            return false;
        }

        public string DisplayName {
            get { return _name; }
        }

        public string TypeName {
            get { return _name; }
        }
    }
}
