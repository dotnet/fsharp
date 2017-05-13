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
using System.ComponentModel;

namespace Microsoft.VisualStudioTools.Project {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class SRDisplayNameAttribute : DisplayNameAttribute {
        string _name;

        public SRDisplayNameAttribute(string name) {
            _name = name;
        }

        public override string DisplayName {
            get {
                return SR.GetString(_name);
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRDescriptionAttribute : DescriptionAttribute {
        private bool _replaced;

        public SRDescriptionAttribute(string description)
            : base(description) {
        }

        public override string Description {
            get {
                if (!_replaced) {
                    _replaced = true;
                    DescriptionValue = SR.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute {
        public SRCategoryAttribute(string category)
            : base(category) {
        }

        protected override string GetLocalizedString(string value) {
            return SR.GetString(value);
        }
    }
}
