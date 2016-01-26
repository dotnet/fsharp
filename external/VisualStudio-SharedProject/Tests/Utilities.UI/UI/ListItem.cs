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
    public class ListItem : AutomationWrapper {
        private ListView _parent;
        private AutomationElementCollection _columns;
        public ListItem(AutomationElement element, ListView parent) : base(element) { 
            _parent = parent;
            _columns = FindAllByControlType(ControlType.Text);
        }
    
        public string this[int index] {
            get {
                Assert.IsNotNull(_columns);
                Assert.IsTrue(0 <= index && index < _columns.Count, "Index {0} is out of range of column count {1}", index, _columns.Count);
                return _columns[index].GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            }
        }

        public string this[string columnName] {
            get {
                Assert.IsNotNull(_parent.Header, "Parent List does not define column headers!");
                return this[_parent.Header[columnName]];
            }
        }
    }
}
