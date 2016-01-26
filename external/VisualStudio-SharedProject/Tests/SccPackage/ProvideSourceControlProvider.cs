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
using Microsoft.VisualStudio.Shell;

namespace Microsoft.TestSccPackage {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ProvideSourceControlProvider : RegistrationAttribute {
        private readonly string _name;
        private readonly Guid _sourceControlGuid;
        private readonly Type _providerType, _packageType;

        public ProvideSourceControlProvider(string friendlyName, string sourceControlGuid, Type sccPackage, Type sccProvider) {
            _name = friendlyName;
            _providerType = sccProvider;
            _packageType = sccPackage;
            _sourceControlGuid = new Guid(sourceControlGuid);
        }

        public override void Register(RegistrationContext context) {
            // http://msdn.microsoft.com/en-us/library/bb165948.aspx
            using (Key sccProviders = context.CreateKey("SourceControlProviders")) {
                using (Key sccProviderKey = sccProviders.CreateSubkey(_sourceControlGuid.ToString("B"))) {
                    sccProviderKey.SetValue("", _name);
                    sccProviderKey.SetValue("Service", _providerType.GUID.ToString("B"));
                        
                    using (Key sccProviderNameKey = sccProviderKey.CreateSubkey("Name")) {
                        sccProviderNameKey.SetValue("", _name);
                        sccProviderNameKey.SetValue("Package", _packageType.GUID.ToString("B"));
                    }
                }
            }/*
            using (Key currentProvider = context.CreateKey("CurrentSourceControlProvider")) {
                currentProvider.SetValue("", _sourceControlGuid.ToString("B"));
            }*/
        }

        public override void Unregister(RegistrationContext context) {
        }
    }
}
