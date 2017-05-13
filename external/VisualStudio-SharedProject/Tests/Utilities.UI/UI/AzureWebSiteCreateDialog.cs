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
    public class AzureWebSiteCreateDialog : AutomationDialog {
        public AzureWebSiteCreateDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public void ClickCreate() {
            // Wait for the create button to be enabled
            WaitFor(CreateButton, btn => btn.Element.Current.IsEnabled);

            // Wait for Locations and Databases to have a selection
            // (the create button may be enabled before they are populated)
            WaitFor(LocationComboBox, combobox => combobox.GetSelectedItemName() != null);
            WaitFor(DatabaseComboBox, combobox => combobox.GetSelectedItemName() != null);

            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(180.0), () => CreateButton.Click());
        }

        public string SiteName {
            get {
                return GetSiteNameBox().GetValuePattern().Current.Value;
            }
            set {
                WaitForInputIdle();
                GetSiteNameBox().GetValuePattern().SetValue(value);
            }
        }

        private Button CreateButton {
            get {
                return new Button(FindByName("Create"));
            }
        }

        private ComboBox LocationComboBox {
            get {
                return new ComboBox(FindByAutomationId("_azureSiteLocation"));
            }
        }

        private ComboBox DatabaseComboBox {
            get {
                return new ComboBox(FindByAutomationId("_azureDatabaseServer"));
            }
        }

        private AutomationElement GetSiteNameBox() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "_azureSiteName"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)
                )
            );
        }
    }
}
