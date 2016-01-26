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
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudioTools {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ProvideDebugPortSupplierAttribute : RegistrationAttribute {
        private readonly string _id, _name;
        private readonly Type _portSupplier, _portPicker;

        public ProvideDebugPortSupplierAttribute(string name, Type portSupplier, string id, Type portPicker = null) {
            _name = name;
            _portSupplier = portSupplier;
            _id = id;
            _portPicker = portPicker;
        }

        public override void Register(RegistrationContext context) {
            var engineKey = context.CreateKey("AD7Metrics\\PortSupplier\\" + _id);
            engineKey.SetValue("Name", _name);
            engineKey.SetValue("CLSID", _portSupplier.GUID.ToString("B"));
            if (_portPicker != null) {
                engineKey.SetValue("PortPickerCLSID", _portPicker.GUID.ToString("B"));
            }

            var clsidKey = context.CreateKey("CLSID");
            var clsidGuidKey = clsidKey.CreateSubkey(_portSupplier.GUID.ToString("B"));
            clsidGuidKey.SetValue("Assembly", _portSupplier.Assembly.FullName);
            clsidGuidKey.SetValue("Class", _portSupplier.FullName);
            clsidGuidKey.SetValue("InprocServer32", context.InprocServerPath);
            clsidGuidKey.SetValue("CodeBase", Path.Combine(context.ComponentPath, _portSupplier.Module.Name));
            clsidGuidKey.SetValue("ThreadingModel", "Free");
        }

        public override void Unregister(RegistrationContext context) {
        }
    }
}
