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
using Microsoft.VisualStudio.Text.Classification;

namespace TestUtilities.Mocks {
    public class MockClassificationType : IClassificationType {
        private readonly string _name;
        private readonly List<IClassificationType> _bases;

        public MockClassificationType(string name, IClassificationType[] bases) {
            _name = name;
            _bases = new List<IClassificationType>(bases);
        }

        public IEnumerable<IClassificationType> BaseTypes {
            get { return _bases; }
        }

        public string Classification {
            get { return _name; }
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

        public void AddBaseType(MockClassificationType mockClassificationType) {
            _bases.Add(mockClassificationType);
        }
    }
}
