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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class ChooseLocationDialog : AutomationDialog {
        public ChooseLocationDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static ChooseLocationDialog FromDte(VisualStudioApp app) {
            return new ChooseLocationDialog(
                app,
                AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("File.ProjectPickerMoveInto"))
            );
        }

        public void SelectProject(string name) {
            var item = Element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
            Assert.IsNotNull(item, "Did not find item " + name);
            item.GetSelectionItemPattern().Select();
        }

    }
}
