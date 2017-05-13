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
using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI
{
    class ProjectPropertiesWindow : AutomationWrapper
    {
        public ProjectPropertiesWindow(IntPtr element)
            : base(AutomationElement.FromHandle(element)) { 
        }

        public AutomationElement this[Guid tabGuid] {
            get {
                
                var tabItem = FindByAutomationId("PropPage_" + tabGuid.ToString("n").ToLower());
                Assert.IsNotNull(tabItem, "Failed to find page");
                
                AutomationWrapper.DumpElement(tabItem);
                foreach (var p in tabItem.GetSupportedPatterns()) {
                    Console.WriteLine("Supports {0}", p.ProgrammaticName);
                }

                try {
                    tabItem.GetInvokePattern().Invoke();
                } catch (InvalidOperationException) {
                    AutomationWrapper.DoDefaultAction(tabItem);
                }

                return FindByAutomationId("PageHostingPanel");
            }
        }
    }
}
