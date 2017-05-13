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
    public class AzureCloudServiceCreateDialog : AutomationDialog {
        public AzureCloudServiceCreateDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public void ClickCreate() {
            // Wait for the create button to be enabled
            WaitFor(OkButton, btn => btn.Element.Current.IsEnabled);

            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(30.0), () => OkButton.Click());
        }

        public string ServiceName {
            get {
                return GetServiceNameBox().GetValuePattern().Current.Value;
            }
            set {
                WaitForInputIdle();
                GetServiceNameBox().GetValuePattern().SetValue(value);
            }
        }

        public string Location {
            get {
                return LocationComboBox.GetSelectedItemName();
            }
            set {
                WaitForInputIdle();
                WaitFor(LocationComboBox, combobox => combobox.GetSelectedItemName() != "<Loading...>");
                LocationComboBox.SelectItem(value);
            }
        }

        private Button OkButton {
            get {
                return new Button(FindByAutomationId("OkButton"));
            }
        }

        private ComboBox LocationComboBox {
            get {
                return new ComboBox(FindByAutomationId("LocationComboBox"));
            }
        }

        private AutomationElement GetServiceNameBox() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "ServiceNameTextBox"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)
                )
            );
        }
    }
}
