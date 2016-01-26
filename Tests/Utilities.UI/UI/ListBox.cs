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
    public class ListBox : AutomationWrapper {
        public ListBox(AutomationElement element)
            : base(element) {
        }

        public ListBoxItem this[int index] {
            get {
                var items = FindAllByControlType(ControlType.ListItem);
                Assert.IsTrue(0 <= index && index < items.Count, "Index {0} is out of range of item count {1}", index, items.Count);
                return new ListBoxItem(items[index], this);
            }
        }

        public int Count {
            get {
                var items = FindAllByControlType(ControlType.ListItem);
                return items.Count;
            }
        }
    }
}
