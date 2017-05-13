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

namespace TestUtilities.UI {
    public class NavigateToDialog : AutomationWrapper, IDisposable {
        public NavigateToDialog(IntPtr hwnd)
            : base(AutomationElement.FromHandle(hwnd)) {
        }

        public NavigateToDialog(AutomationElement element)
            : base(element) {
        }

        public void Dispose() {
            Close();
        }

        public void GoToSelection() {
#if DEV12_OR_LATER
            ClickButtonByAutomationId("PART_SearchButton");
#else
            ClickButtonByAutomationId("okButton");
#endif
        }

        public void Close() {
#if DEV12_OR_LATER
            GetSearchBox().SetFocus();
            Keyboard.PressAndRelease(System.Windows.Input.Key.Escape);
#else
            ClickButtonByAutomationId("cancelButton");
#endif
        }

        public string SearchTerm {
            get {
                var term = (ValuePattern)GetSearchBox().GetCurrentPattern(ValuePattern.Pattern);
                return term.Current.Value;
            }
            set {
                var term = (ValuePattern)GetSearchBox().GetCurrentPattern(ValuePattern.Pattern);
                term.SetValue(string.Empty);
                GetSearchBox().SetFocus();
                Keyboard.Type(value);
            }
        }

        internal AutomationElement GetSearchBox() {
#if DEV12_OR_LATER
            return Element.FindFirst(TreeScope.Descendants, new AndCondition(
                new PropertyCondition(AutomationElement.AutomationIdProperty, "PART_SearchBox"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "TextBox")
            ));
#else
            return Element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "searchTerms")
            ).FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ClassNameProperty, "Edit")
            );
#endif
        }

#if DEV12_OR_LATER
        private AutomationElement GetResultsList() {
            return Element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "PART_ResultList")
            );
        }

        internal int WaitForNumberOfResults(int results) {
            for (int retries = 10; retries > 0; --retries) {
                var list = GetResultsList();
                if (list != null) {
                    var count = list.FindAll(TreeScope.Children, Condition.TrueCondition).Count;
                    if (count >= results) {
                        return count;
                    }
                }
                Thread.Sleep(1000);
            }

            return 0;
        }
#else
        private GridPattern GetResultsList() {
            return (GridPattern)Element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "results")
            ).GetCurrentPattern(GridPattern.Pattern);
        }

        internal int WaitForNumberOfResults(int results) {
            var list = GetResultsList();

            for (int count = 10; count > 0 && list.Current.RowCount < results; --count) {
                Thread.Sleep(1000);
            }
            return list.Current.RowCount;
        }
#endif
    }
}
