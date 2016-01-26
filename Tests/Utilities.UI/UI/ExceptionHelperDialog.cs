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

using System.Windows.Automation;

namespace TestUtilities.UI {
    public class ExceptionHelperDialog : AutomationWrapper {
        public ExceptionHelperDialog(AutomationElement element)
            : base(element) {
        }

        public string Title {
            get {
                // this is just the 1st child pane, and it's name is the same as the text it has.
                var exceptionNamePane = Element.FindFirst(
                    TreeScope.Children,
                    new PropertyCondition(
                        AutomationElement.ControlTypeProperty,
                        ControlType.Pane
                    )
                );

                return exceptionNamePane.Current.Name;
            }
        }

        public string Description {
            get {
                var desc = FindByName("Exception Description:");
                return (((TextPattern)desc.GetCurrentPattern(TextPattern.Pattern)).DocumentRange.GetText(-1).ToString());
            }
            
        }

        public void Cancel() {
            ClickButtonByName("Cancel Button");
        }
    }
}
