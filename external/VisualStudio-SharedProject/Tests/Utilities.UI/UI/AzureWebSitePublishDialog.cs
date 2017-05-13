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
    public class AzureWebSitePublishDialog : AutomationDialog {
        public AzureWebSitePublishDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static AzureWebSitePublishDialog FromDte(VisualStudioApp app) {
            var publishDialogHandle = app.OpenDialogWithDteExecuteCommand("Build.PublishSelection");
            return new AzureWebSitePublishDialog(app, AutomationElement.FromHandle(publishDialogHandle));
        }

        public AzureWebSiteImportPublishSettingsDialog ClickImportSettings() {
            WaitForInputIdle();
            ClickButtonByAutomationId("ImportSettings");
            return new AzureWebSiteImportPublishSettingsDialog(App, AutomationElement.FromHandle(App.WaitForDialogToReplace(Element)));
        }

        public void ClickPublish() {
            WaitForInputIdle();
            WaitForClosed(TimeSpan.FromSeconds(10.0), () => ClickButtonByAutomationId("PublishButton"));
        }
    }
}
