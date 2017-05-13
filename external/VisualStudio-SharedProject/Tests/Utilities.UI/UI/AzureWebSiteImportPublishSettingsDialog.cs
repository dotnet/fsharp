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
    public class AzureWebSiteImportPublishSettingsDialog : AutomationDialog {
        public AzureWebSiteImportPublishSettingsDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public void ClickImportFromWindowsAzureWebSite() {
            WaitForInputIdle();
            ImportFromWindowsAzureWebSiteRadioButton().Select();
        }

        public void ClickSignOut() {
            WaitForInputIdle();
            var sign = new AutomationWrapper(FindByAutomationId("AzureSigninControl"));
            sign.ClickButtonByName("Sign Out");
        }

        public AzureManageSubscriptionsDialog ClickImportOrManageSubscriptions() {
            WaitForInputIdle();
            var importElement = ImportSubscriptionsHyperlink();
            if (importElement == null) {
                importElement = ManageSubscriptionsHyperlink();
            }
            importElement.GetInvokePattern().Invoke();
            return new AzureManageSubscriptionsDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public AzureWebSiteCreateDialog ClickNew() {
            WaitForInputIdle();
            ClickButtonByAutomationId("NewButton");
            return new AzureWebSiteCreateDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public void ClickOK() {
            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(10.0), () => ClickButtonByAutomationId("OKButton"));
        }

        private AutomationElement ImportFromWindowsAzureWebSiteRadioButton() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "ImportLabel"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.RadioButton)
                )
            );
        }

        private AutomationElement ImportSubscriptionsHyperlink() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "Import subscriptions"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink)
                )
            );
        }

        private AutomationElement ManageSubscriptionsHyperlink() {
            return Element.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.NameProperty, "Manage subscriptions"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink)
                )
            );
        }
    }
}
