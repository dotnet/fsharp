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
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Provides access to creating or finding existing REPL windows.   
    /// </summary>
    public interface IReplWindowProvider {
        /// <summary>
        /// Creates a REPL window and returns a ToolWindowPane which implements IReplWindow.  An IReplEvaluatorProvider must exist
        /// to respond and create the specified REPL ID.
        /// 
        /// The returned object is also a ToolWindowPane and can be cast for access to control the docking with VS.
        /// </summary>
        IReplWindow CreateReplWindow(IContentType/*!*/ contentType, string/*!*/ title, Guid languageServiceGuid, string replId);

        /// <summary>
        /// Finds the REPL w/ the specified ID or returns null if the window hasn't been created.  An IReplEvaluatorProvider must exist
        /// to respond and create the specified REPL ID.
        /// 
        /// The returned object is also a ToolWindowPane and can be cast for access to control the docking with VS.
        /// </summary>
        IReplWindow FindReplWindow(string replId);

        /// <summary>
        /// Returns this list of repl windows currently loaded.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IReplWindow> GetReplWindows();
    }
}
