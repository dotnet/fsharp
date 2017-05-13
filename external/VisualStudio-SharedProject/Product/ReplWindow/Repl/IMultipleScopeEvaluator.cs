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

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Supports a REPL evaluator which enables the user to switch between
    /// multiple scopes of execution.
    /// </summary>
    public interface IMultipleScopeEvaluator : IReplEvaluator {
        /// <summary>
        /// Sets the current scope to the given name.
        /// </summary>
        void SetScope(string scopeName);

        /// <summary>
        /// Gets the list of scopes which can be changed to.
        /// </summary>
        IEnumerable<string> GetAvailableScopes();

        /// <summary>
        /// Gets the current scope name.
        /// </summary>
        string CurrentScopeName {
            get;
        }

        /// <summary>
        /// Event is fired when the list of available scopes changes.
        /// </summary>
        event EventHandler<EventArgs> AvailableScopesChanged;

        /// <summary>
        /// Event is fired when support of multiple scopes has changed.
        /// </summary>
        event EventHandler<EventArgs> MultipleScopeSupportChanged;

        /// <summary>
        /// Returns true if multiple scope support is currently enabled, false if not.
        /// </summary>
        bool EnableMultipleScopes {
            get;
        }
    }
}
