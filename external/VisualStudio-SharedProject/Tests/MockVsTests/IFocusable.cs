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


namespace Microsoft.VisualStudioTools.MockVsTests {
    /// <summary>
    /// Implemented by mock VS objects which can gain and lose focus.
    /// 
    /// Only one item in mock VS will have focus at a time, and the
    /// current item is tracked by MockVs.
    /// </summary>
    interface IFocusable {
        void GetFocus();
        void LostFocus();
    }
}
