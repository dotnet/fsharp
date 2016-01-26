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
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudioTools.MockVsTests {
    /// <summary>
    /// Stores cached state for creating a MockVs.  This state is initialized once for the process and then
    /// re-used to create new MockVs instances.  We create fresh MockVs instances to avoid having state
    /// lingering between tests.
    /// </summary>
    class CachedVsInfo {
        public readonly AggregateCatalog Catalog;
        public readonly List<Type> Packages;
        public Dictionary<string, LanguageServiceInfo> LangServicesByName = new Dictionary<string, LanguageServiceInfo>();
        public Dictionary<Guid, LanguageServiceInfo> LangServicesByGuid = new Dictionary<Guid, LanguageServiceInfo>();
        public Dictionary<string, string> _languageNamesByExtension = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public CachedVsInfo(AggregateCatalog catalog, List<Type> packages) {
            Catalog = catalog;
            Packages = packages;

            foreach (var package in Packages) {
                var attrs = package.GetCustomAttributes(typeof(ProvideLanguageServiceAttribute), false);
                foreach (ProvideLanguageServiceAttribute attr in attrs) {
                    foreach (var type in package.Assembly.GetTypes()) {
                        if (type.GUID == attr.LanguageServiceSid) {
                            var info = new LanguageServiceInfo(attr);
                            LangServicesByGuid[attr.LanguageServiceSid] = info;
                            LangServicesByName[attr.LanguageName] = info;

                            break;
                        }
                    }
                }

                var extensions = package.GetCustomAttributes(typeof(ProvideLanguageExtensionAttribute), false);
                foreach (ProvideLanguageExtensionAttribute attr in extensions) {
                    LanguageServiceInfo info;
                    if (LangServicesByGuid.TryGetValue(attr.LanguageService, out info)) {
                        _languageNamesByExtension[attr.Extension] = info.Attribute.LanguageName;
                    }
                }
            }
        }
    }
}
