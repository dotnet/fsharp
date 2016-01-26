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
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    /// Base class for available commands.  To add a new command you must first update the .vsct file so that
    /// our commands are registered and available.  Then you need to subclass Command and at a new instance of
    /// the command in CommandTable.  PythonToolsPackage will then register the command on startup.
    /// </summary>
    internal abstract class Command {
        /// <summary>
        /// Provides the implementation of what should happen when the command is executed.
        /// 
        /// sender is the MenuCommand or OleMenuCommand object which is causing the event to be fired.
        /// </summary>
        public abstract void DoCommand(object sender, EventArgs args);

        /// <summary>
        /// Enables a command to hook into our edit filter for Python text buffers.
        /// 
        /// Called with the OLECMD object for the command being processed.  Returns null
        /// if the command does not want to handle this message or the HRESULT that
        /// should be returned from the QueryStatus call.
        /// </summary>
        public virtual int? EditFilterQueryStatus(ref OLECMD cmd, IntPtr pCmdText) {
            return null;
        }

        /// <summary>
        /// Provides the CommandId for this command which corresponds to the CommandId in the vsct file
        /// and PkgCmdId.cs.
        /// </summary>
        public abstract int CommandId {
            get;
        }

        /// <summary>
        /// Provides an event handler that will be invoked before the menu containing the command
        /// is displayed.  This can enable, disable, or hide the menu command.  By default returns
        /// null.
        /// </summary>
        public virtual EventHandler BeforeQueryStatus {
            get {
                return null;
            }
        }
    }
}
