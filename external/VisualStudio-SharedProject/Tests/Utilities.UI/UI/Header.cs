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

using System.Collections.Generic;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class Header : AutomationWrapper {
        private Dictionary<string, int> _columns = new Dictionary<string, int>();
        public Dictionary<string, int> Columns {
            get {
                return _columns;
            }
        }

        public Header(AutomationElement element) : base(element) {
            AutomationElementCollection headerItems = FindAllByControlType(ControlType.HeaderItem);
            for (int i = 0; i < headerItems.Count; i++) {
                string colName = headerItems[i].GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
                if (colName != null && !_columns.ContainsKey(colName)) _columns[colName] = i;
            }            
        }

        public int this[string colName] {
            get {
                Assert.IsTrue(_columns.ContainsKey(colName), "Header does not define header item {0}", colName);
                return _columns[colName];
            }
        }
        
    }
}
