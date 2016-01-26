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
using System.Threading.Tasks;
using System.Windows.Automation;
using Microsoft.VisualStudio.Shell.Interop;

namespace TestUtilities.UI {
    public class AzureCloudServicePublishDialog : AutomationDialog {
        public AzureCloudServicePublishDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static AzureCloudServicePublishDialog FromDte(VisualStudioApp app) {
            var publishDialogHandle = app.OpenDialogWithDteExecuteCommand("Build.PublishSelection");
            return new AzureCloudServicePublishDialog(app, AutomationElement.FromHandle(publishDialogHandle));
        }

        public AzureManageSubscriptionsDialog SelectManageSubscriptions() {
            WaitForInputIdle();

            // <Manage...> is different from other list item, selecting it 
            // using SelectItem will throw COMException
            // This is what the cloud service team did in their tests for their 
            // combo box special items that bring up dialogs.
            AccountComboBox.ClickItem("<Manage...>");

            return new AzureManageSubscriptionsDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public AzureCloudServiceCreateDialog SelectCreateNewService() {
            WaitForInputIdle();

            // <Create New...> is different from other list item, selecting it 
            // using SelectItem will throw COMException
            // This is what the cloud service team did in their tests for their 
            // combo box special items that bring up dialogs.
            ServiceComboBox.ClickItem("<Create New...>");

            return new AzureCloudServiceCreateDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public void ClickPublish() {
            // Wait for the publish button to be enabled
            // It will not be enabled until all combo boxes have a valid
            // selection, such as the storage account combo box.
            WaitFor(PublishButton, btn => btn.Element.Current.IsEnabled);

            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(10.0), () => PublishButton.Click());
        }

        public void ClickNext() {
            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(10.0), () => ClickButtonByAutomationId("PART_NextCommandSource"));
        }

        private Button PublishButton {
            get {
                return new Button(FindByAutomationId("PART_FinishCommandSource"));
            }
        }

        private ComboBox AccountComboBox {
            get {
                return new ComboBox(FindByAutomationId("accountCombo"));
            }
        }

        private ComboBox ServiceComboBox {
            get {
                return new ComboBox(FindByAutomationId("ServiceComboBox"));
            }
        }
    }
}
