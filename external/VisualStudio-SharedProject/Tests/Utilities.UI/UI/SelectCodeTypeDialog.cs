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
    public class SelectCodeTypeDialog : AutomationWrapper {
        private CheckListView _availableCodeTypes;

        public SelectCodeTypeDialog(AutomationElement element) : base(element) {
            var actElement = Element.FindFirst(
                           TreeScope.Descendants,
                           new PropertyCondition(
                               AutomationElement.AutomationIdProperty,
                               "1005")); // AutomationId 1005 discovered with UISpy
            _availableCodeTypes = new CheckListView(actElement);
        }

        public SelectCodeTypeDialog(IntPtr hwnd) : this(AutomationElement.FromHandle(hwnd)) { }

        public CheckListView AvailableCodeTypes {
            get {
                return _availableCodeTypes;
            }
        }

        public void SetDebugSpecificCodeTypes() {
            Select(FindByAutomationId("1199")); // utomationId 1199 discovered with UISpy
        }

        public void SetAutomaticallyDetermineCodeTypes() {
            Select(FindByAutomationId("1196")); // AutomationId 1196
        }

        public CheckBox GetCodeTypeCheckBox(string codeType) {
            var selectedItem = _availableCodeTypes.GetFirstByName(codeType);
            return selectedItem;
        }

        public void ClickOk() {
            ClickButtonByName("OK");
        }

        public void ClickCancel() {
            ClickButtonByName("Cancel");
        }

    }
}
