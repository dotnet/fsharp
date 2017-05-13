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
using System.Collections.Generic;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class CheckListView : AutomationWrapper {
        private List<CheckBox> _items;
        private Header _header;

        public Header Header {
            get {
                if (_header == null) {
                    var headerel = FindFirstByControlType(ControlType.Header);
                    if (headerel != null)
                        _header = new Header(FindFirstByControlType(ControlType.Header));
                }
                return _header;
            }
        }

        public List<CheckBox> Items {
            get {
                if (_items == null) {
                    _items = new List<CheckBox>();
                    AutomationElementCollection rawItems = FindAllByControlType(ControlType.CheckBox);
                    foreach (AutomationElement el in rawItems) {
                        _items.Add(new CheckBox(el, this));
                    }
                }
                return _items;
            }
        }

        public CheckListView(AutomationElement element) : base(element) { }

        public CheckBox GetFirstByName(string name) {
            foreach (CheckBox r in Items) {
                if (r.Name.Equals(name, StringComparison.CurrentCulture)) return r;
            }
            Assert.Fail("No item found with Name == {0}", name);
            return null;
        }

    }
}
