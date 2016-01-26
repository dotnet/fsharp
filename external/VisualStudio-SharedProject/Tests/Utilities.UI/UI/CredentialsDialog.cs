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
using System.Windows.Automation;

namespace TestUtilities.UI {
    class CredentialsDialog : AutomationDialog {
        public CredentialsDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static CredentialsDialog PublishSelection(VisualStudioApp app) {
            return new CredentialsDialog(
                app,
                AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("Build.PublishSelection"))
            );
        }

        public string UserName {
            get {
                return GetUsernameEditBox().GetValuePattern().Current.Value;
            }
            set {
                GetUsernameEditBox().GetValuePattern().SetValue(value);
            }
        }

        public string Password {
            get {
                return GetPasswordEditBox().GetValuePattern().Current.Value;
            }
            set {
                GetPasswordEditBox().GetValuePattern().SetValue(value);
            }
        }

        private AutomationElement GetUsernameEditBox() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "User name:"),
                    new PropertyCondition(AutomationElement.ClassNameProperty, "Edit")
                )
            );
        }

        private AutomationElement GetPasswordEditBox() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "Password:"),
                    new PropertyCondition(AutomationElement.ClassNameProperty, "Edit")
                )
            );
        }
    }
}
