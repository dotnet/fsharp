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
    /// Wraps the Delete/Remove/Cancel dialog displayed when removing something from a hierarchy window (such as the solution explorer).
    /// </summary>
    public class RemoveItemDialog : AutomationDialog {
        public RemoveItemDialog(IntPtr hwnd)
            : base(null, AutomationElement.FromHandle(hwnd)) {
        }

        public RemoveItemDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static RemoveItemDialog FromDte(VisualStudioApp app) {
            return new RemoveItemDialog(app, AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("Edit.Delete")));
        }

        public override void OK() {
            throw new NotSupportedException();
        }

        public void Remove() {
            WaitForInputIdle();
            WaitForClosed(DefaultTimeout, () => ClickButtonByName("Remove"));
        }

        public void Delete() {
            WaitForInputIdle();
            WaitForClosed(DefaultTimeout, () => ClickButtonByName("Delete"));
        }
    }
}
