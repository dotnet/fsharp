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
    /// Wrapps VS's Add Reference Dialog
    /// </summary>
    class AddReferenceDialog  : AutomationWrapper {
        public AddReferenceDialog(AutomationElement element)
            : base(element) {
        }

        /// <summary>
        /// Clicks the OK button on the dialog.
        /// </summary>
        public void ClickOK() {
            ClickButtonByName("OK");
        }

        public void ActivateBrowseTab() {
            for (int i = 0; i < 20; i++) {
                var tabItem = Element.FindFirst(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(
                            AutomationElement.ControlTypeProperty,
                            ControlType.TabItem
                        ),
                        new PropertyCondition(
                            AutomationElement.NameProperty,
                            "Browse"
                        )
                    )
                );
                if (tabItem == null) {
                    System.Threading.Thread.Sleep(500);
                    continue;
                }

                var pattern = (SelectionItemPattern)tabItem.GetCurrentPattern(SelectionItemPattern.Pattern);
                pattern.Select();
            }
        }

        public string BrowseFilename {
            get {
                return GetFilenameValuePattern().Current.Value;
            }
            set {
                GetFilenameValuePattern().SetValue(value);
            }
        }

        private ValuePattern GetFilenameValuePattern() {
            var filename = Element.FindFirst(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(
                        AutomationElement.ControlTypeProperty,
                        ControlType.Edit
                    ),
                    new PropertyCondition(
                        AutomationElement.NameProperty,
                        "File name:"
                    )
                )
            );

            return (ValuePattern)filename.GetCurrentPattern(ValuePattern.Pattern);
        }
    }
}
