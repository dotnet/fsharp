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

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Creates a REPL window.  Implementations should check replId and ensure that it is a REPL window that they requested to be created.  The
    /// replId which will be provided is the same as the ID passed to IReplWindowProvider.CreateReplWindow.  You can receive an ID which has
    /// not been created during the current Visual Studio session if the user exited Visual Studio with the REPL window opened and docked.  Therefore
    /// the replId should contain enough information to re-create the appropriate REPL window.
    /// </summary>
    public interface IReplEvaluatorProvider {
        IReplEvaluator GetEvaluator(string replId);
    }
}
