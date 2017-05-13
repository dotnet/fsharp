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
    public class Table : AutomationWrapper {
        private readonly GridPattern _pattern;

        public Table(AutomationElement element)
            : base(element) {
                _pattern = (GridPattern)element.GetCurrentPattern(GridPattern.Pattern);

        }

        public AutomationElement this[int row, int column] {
            get {
                return _pattern.GetItem(row, column);
            }
        }

        public AutomationElement FindItem(string name) {
            return Element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, name));
        }
    }
}
