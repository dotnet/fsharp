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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace TestUtilities.Mocks {
    [Export(typeof(IContentTypeRegistryService))]
    public class MockContentTypeRegistryService : IContentTypeRegistryService {
        private readonly Dictionary<string, MockContentType> _contentTypes = new Dictionary<string, MockContentType>(StringComparer.InvariantCultureIgnoreCase);
        private const string _unknownName = "UNKNOWN";
        private readonly MockContentType _unknownType;
        private static string[] Empty = new string[0];

        public MockContentTypeRegistryService() {
            _contentTypes[_unknownName] = _unknownType = new MockContentType(_unknownName, new IContentType[0]);
        }

        public MockContentTypeRegistryService(params string[] existingNames) : this() {
            foreach (var type in existingNames) {
                AddContentType(type, new string[0]);
            }
        }

        #region IContentTypeRegistryService Members

        public IContentType AddContentType(string typeName, IEnumerable<string> baseTypeNames) {
            if (typeName == null) {
                throw new ArgumentNullException();
            }

            string uppercaseTypeName = typeName.ToUpperInvariant();
            if (uppercaseTypeName == _unknownName) {
                throw new InvalidOperationException();
            }
 

            MockContentType type;
            if (!_contentTypes.TryGetValue(typeName, out type)) {
                _contentTypes[typeName] = new MockContentType(typeName, new IContentType[0]);
            }
            List<IContentType> baseTypes = new List<IContentType>();
            foreach (var baseTypeName in baseTypeNames) {
                baseTypes.Add(AddContentType(baseTypeName, Empty));
            }
            return type;
        }

        public IEnumerable<IContentType> ContentTypes {
            get { return _contentTypes.Values; }
        }

        public IContentType GetContentType(string typeName) {
            MockContentType res;
            if (_contentTypes.TryGetValue(typeName, out res)) {
                return res;
            }
            throw new InvalidOperationException("Unknown content type: " + typeName);
        }

        public void RemoveContentType(string typeName) {
            string uppercaseTypeName = typeName.ToUpperInvariant();
            if (uppercaseTypeName == _unknownName) {
                throw new InvalidOperationException();
            }
            _contentTypes.Remove(typeName);
        }

        public IContentType UnknownContentType {
            get { return _unknownType; }
        }

        #endregion
    }
}
