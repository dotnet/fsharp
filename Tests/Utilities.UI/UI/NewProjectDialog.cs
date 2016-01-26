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
    /// <summary>
    /// Wrapps VS's Project->Add Item dialog.
    /// </summary>
    public class NewItemDialog : AutomationDialog, IAddNewItem {
        private readonly VisualStudioApp _app;
        private Table _projectTypesTable;

        public NewItemDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
            _app = app;
        }

        public static NewItemDialog FromDte(VisualStudioApp app) {
            return new NewItemDialog(
                app,
                AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("Project.AddNewItem"))
            );
        }

        /// <summary>
        /// Clicks the OK button on the dialog.
        /// </summary>
        public override void OK() {
            ClickButtonAndClose("btn_OK", nameIsAutomationId: true);
        }

        /// <summary>
        /// Gets the project types table which enables selecting an individual project type.
        /// </summary>
        public Table ProjectTypes {
            get {
                if (_projectTypesTable == null) {
                    var extensions = Element.FindAll(
                        TreeScope.Descendants,
                        new PropertyCondition(
                            AutomationElement.AutomationIdProperty,
                            "lvw_Extensions"
                        )
                    );

                    if (extensions.Count != 1) {
                        throw new Exception("multiple controls match");
                    }
                    _projectTypesTable = new Table(extensions[0]);

                }
                return _projectTypesTable;
            }
        }

        public string FileName {
            get {
                var patterns = GetFileNameBox().GetSupportedPatterns();
                var filename = (ValuePattern)GetFileNameBox().GetCurrentPattern(ValuePattern.Pattern);
                return filename.Current.Value;
            }
            set {
                var patterns = GetFileNameBox().GetSupportedPatterns();
                var filename = (ValuePattern)GetFileNameBox().GetCurrentPattern(ValuePattern.Pattern);
                filename.SetValue(value);
            }
        }

        private AutomationElement GetFileNameBox() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "txt_Name"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)
                )
            );
        }
    }
}
