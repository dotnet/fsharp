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
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;

namespace TestUtilities.Mocks {
    [Export(typeof(IClassificationTypeRegistryService))]
    public class MockClassificationTypeRegistryService : IClassificationTypeRegistryService {
        static Dictionary<string, MockClassificationType> _types = new Dictionary<string, MockClassificationType>();

        public MockClassificationTypeRegistryService() {
            foreach (FieldInfo fi in typeof(PredefinedClassificationTypeNames).GetFields()) {
                string name = (string)fi.GetValue(null);
                _types[name] = new MockClassificationType(name, new IClassificationType[0]);
            }
        }

        [ImportingConstructor]
        public MockClassificationTypeRegistryService([ImportMany]IEnumerable<Lazy<ClassificationTypeDefinition, IClassificationTypeDefinitionMetadata>> classTypeDefs)
            : this() {
            foreach (var def in classTypeDefs) {
                string name = def.Metadata.Name;
                var type = GetClasificationType(name);
                foreach (var baseType in def.Metadata.BaseDefinition ?? Enumerable.Empty<string>()) {
                    type.AddBaseType(GetClasificationType(baseType));
                }
            }
        }

        private static MockClassificationType GetClasificationType(string name) {
            MockClassificationType type;
            if (!_types.TryGetValue(name, out type)) {
                _types[name] = type = new MockClassificationType(name, new IClassificationType[0]);
            }
            return type;
        }

        public IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes) {
            return _types[type] = new MockClassificationType(type, baseTypes.ToArray());
        }

        public IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes) {
            return new MockClassificationType(String.Empty, baseTypes);
        }

        public IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes) {
            return new MockClassificationType(String.Empty, baseTypes.ToArray());
        }

        public IClassificationType GetClassificationType(string type) {
            MockClassificationType result;
            return _types.TryGetValue(type, out result) ? result : null;
        }
    }
}
