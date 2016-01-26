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

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudioTools {
    class ProvideDebugLanguageAttribute : RegistrationAttribute {
        private readonly string _languageGuid, _languageName, _engineGuid, _eeGuid;

        public ProvideDebugLanguageAttribute(string languageName, string languageGuid, string eeGuid, string debugEngineGuid) {
            _languageName = languageName;
            _languageGuid = languageGuid;
            _eeGuid = eeGuid;
            _engineGuid = debugEngineGuid;
        }

        public override void Register(RegistrationContext context) {
            var langSvcKey = context.CreateKey("Languages\\Language Services\\" + _languageName + "\\Debugger Languages\\" + _languageGuid);
            langSvcKey.SetValue("", _languageName);
            // 994... is the vendor ID (Microsoft)
            var eeKey = context.CreateKey("AD7Metrics\\ExpressionEvaluator\\" + _languageGuid + "\\{994B45C4-E6E9-11D2-903F-00C04FA302A1}");
            eeKey.SetValue("Language", _languageName);
            eeKey.SetValue("Name", _languageName);
            eeKey.SetValue("CLSID", _eeGuid);

            var engineKey = eeKey.CreateSubkey("Engine");
            engineKey.SetValue("0", _engineGuid);
        }

        public override void Unregister(RegistrationContext context) {
        }
    }
}
