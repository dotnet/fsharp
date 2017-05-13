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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class CheckBox : AutomationWrapper {
        public string Name { get; set; }

        public CheckBox(AutomationElement element, CheckListView parent)
            : base(element) {
            Name = (string)Element.GetCurrentPropertyValue(AutomationElement.NameProperty);
        }

        public void SetSelected() {
            Assert.IsTrue((bool)Element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty), "Element is not a check box");
            TogglePattern pattern = (TogglePattern)Element.GetCurrentPattern(TogglePattern.Pattern);

            if (pattern.Current.ToggleState != ToggleState.On) pattern.Toggle();
            if (pattern.Current.ToggleState != ToggleState.On) pattern.Toggle();

            Assert.AreEqual(pattern.Current.ToggleState, ToggleState.On, "Could not toggle " + Name + " to On.");
        }

        public void SetUnselected() {
            Assert.IsTrue((bool)Element.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty), "Element is not a check box");
            TogglePattern pattern = (TogglePattern)Element.GetCurrentPattern(TogglePattern.Pattern);

            if (pattern.Current.ToggleState != ToggleState.Off) pattern.Toggle();
            if (pattern.Current.ToggleState != ToggleState.Off) pattern.Toggle();
            Assert.AreEqual(pattern.Current.ToggleState, ToggleState.Off, "Could not toggle " + Name + " to Off.");
        }
    }
}
