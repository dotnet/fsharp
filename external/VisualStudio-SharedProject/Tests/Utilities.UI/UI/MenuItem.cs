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
using System.Windows.Automation;

namespace TestUtilities.UI
{
    class MenuItem : AutomationWrapper
    {
        public MenuItem(AutomationElement element)
            : base(element) {
        }

        public string Value
        {
            get
            {
                return this.Element.Current.Name.ToString();
            }
        }

        public bool ToggleStatus
        {
            get
            {
                var pat = (TogglePattern)Element.GetCurrentPattern(TogglePattern.Pattern);
                if (pat.Current.ToggleState == ToggleState.On)
                    return true;
                return false;
            }
        }

        public void Check()
        {
            var pat = (TogglePattern)Element.GetCurrentPattern(TogglePattern.Pattern);
            if (pat.Current.ToggleState == ToggleState.Off)
            {
                try
                {
                    pat.Toggle();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
        }

        public void Uncheck()
        {
            var pat = (TogglePattern)Element.GetCurrentPattern(TogglePattern.Pattern);
            if (pat.Current.ToggleState == ToggleState.On)
            {
                try
                {
                    pat.Toggle();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
        }
    }
}
