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
    public class ToolsOptionsDialog : AutomationDialog {
        public ToolsOptionsDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static ToolsOptionsDialog FromDte(VisualStudioApp app) {
            return new ToolsOptionsDialog(
                app,
                AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("Tools.Options"))
            );
        }

        public override void OK() {
            ClickButtonAndClose("1", nameIsAutomationId: true);
        }

        public override void Cancel() {
            ClickButtonAndClose("2", nameIsAutomationId: true);
        }

        public string SelectedView {
            set {
                var treeView = new TreeView(Element.FindFirst(
                    TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "SysTreeView32")
                ));

                treeView.FindItem(value.Split('\\', '/')).SetFocus();
            }
        }
    }
}
