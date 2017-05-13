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
using System.Threading;
using System.Windows.Automation;
using System.Windows.Input;

namespace TestUtilities.UI {
    public class SaveDialog : AutomationDialog {
        public SaveDialog(VisualStudioApp app, AutomationElement element)
            : base(app, element) {
        }

        public static SaveDialog FromDte(VisualStudioApp app) {
            return new SaveDialog(
                app,
                AutomationElement.FromHandle(app.OpenDialogWithDteExecuteCommand("File.SaveSelectedItemsAs"))
            );
        }

        public void Save() {
            WaitForInputIdle();
            // The Save button on this dialog is broken and so UIA cannot invoke
            // it (though somehow Inspect is able to...). We use the keyboard
            // instead.
            WaitForClosed(DefaultTimeout, () => Keyboard.PressAndRelease(Key.S, Key.LeftAlt));
        }

        public override void OK() {
            Save();
        }

        public string FileName { 
            get {
                return GetFilenameEditBox().GetValuePattern().Current.Value;
            }
            set {
                GetFilenameEditBox().GetValuePattern().SetValue(value);
            }
        }

        private AutomationElement GetFilenameEditBox() {
            return FindByAutomationId("FileNameControlHost");
        }
    }
}
