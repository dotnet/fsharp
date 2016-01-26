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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudioTools.Navigation {
    /// <summary>
    /// This interface defines the service that finds current language files inside a hierarchy
    /// and builds information to expose to the class view or object browser.
    /// </summary>    
    internal interface ILibraryManager {
        void RegisterHierarchy(IVsHierarchy hierarchy);
        void UnregisterHierarchy(IVsHierarchy hierarchy);
        void RegisterLineChangeHandler(uint document, TextLineChangeEvent lineChanged, Action<IVsTextLines> onIdle);
    }
    internal delegate void TextLineChangeEvent(object sender, TextLineChange[] changes, int last);
}
