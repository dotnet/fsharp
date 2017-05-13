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

namespace TestUtilities.UI
{
    class Menu : AutomationWrapper
    {
        public Menu(AutomationElement element)
            : base(element) {
        }

        public List<MenuItem> Items
        {
            get
            {
                Condition con = new PropertyCondition(
                                    AutomationElement.LocalizedControlTypeProperty,
                                    "menu item"
                                );
                AutomationElementCollection ell = Element.FindAll(TreeScope.Children, con);
                List<MenuItem> items = new List<MenuItem>();
                for (int i = 0; i < ell.Count; i++)
                {
                    items.Add(new MenuItem(ell[i]));
                }
                return items;
            }
        }
    }
}
