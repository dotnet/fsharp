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
    public class AzureManageSubscriptionsDialog : AutomationDialog {
        public AzureManageSubscriptionsDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public void ClickCertificates() {
            WaitForInputIdle();
            CertificatesTab().Select();
        }

        public AzureImportSubscriptionDialog ClickImport() {
            WaitForInputIdle();
            ClickButtonByAutomationId("ImportButton");

            return new AzureImportSubscriptionDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public void ClickRemove() {
            WaitForInputIdle();
            var button = new Button(FindByAutomationId("DeleteButton"));
            WaitFor(button, btn => btn.Element.Current.IsEnabled);
            button.Click();
        }

        public void Close() {
            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(10.0), () => ClickButtonByName("Close"));
        }

        public ListBox SubscriptionsListBox {
            get {
                return new ListBox(FindByAutomationId("SubscriptionsListBox"));
            }
        }

        private AutomationElement CertificatesTab() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "CertificatesTab"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem)
                )
            );
        }
    }
}
