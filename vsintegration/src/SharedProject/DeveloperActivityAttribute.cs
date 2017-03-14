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

namespace Microsoft.VisualStudioTools {
    class DeveloperActivityAttribute : RegistrationAttribute {
        private readonly Type _projectType;
        private readonly int _templateSet;
        private readonly string _developerActivity;

        public DeveloperActivityAttribute(string developerActivity, Type projectPackageType) {
            _developerActivity = developerActivity;
            _projectType = projectPackageType;
            _templateSet = 1;
        }

        public DeveloperActivityAttribute(string developerActivity, Type projectPackageType, int templateSet) {
            _developerActivity = developerActivity;
            _projectType = projectPackageType;
            _templateSet = templateSet;
        }

        public override void Register(RegistrationAttribute.RegistrationContext context) {
            var key = context.CreateKey("NewProjectTemplates\\TemplateDirs\\" + _projectType.GUID.ToString("B") + "\\/" + _templateSet);
            key.SetValue("DeveloperActivity", _developerActivity);
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context) {
        }
    }
}
