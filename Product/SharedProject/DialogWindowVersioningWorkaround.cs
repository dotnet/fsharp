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

using Microsoft.VisualStudio.PlatformUI;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    /// Works around an issue w/ DialogWindow and targetting multiple versions of VS.
    /// 
    /// Because the Microsoft.VisualStudio.Shell.version.0 assembly changes names
    /// we cannot refer to both v10 and v11 versions from within the same XAML file.
    /// Instead we use this subclass defined in our assembly.
    /// </summary>
    class DialogWindowVersioningWorkaround : DialogWindow {
    }
}
