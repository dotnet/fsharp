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

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Defines an interface for launching a project or a file with or without debugging.
    /// </summary>
    public interface IProjectLauncher {
        /// <summary>
        /// Starts a project with or without debugging.
        /// </summary>
        /// <returns>HRESULT indicating success or failure.</returns>
        int LaunchProject(bool debug);

        /// <summary>
        /// Starts a file in a project with or without debugging.
        /// </summary>
        /// <returns>HRESULT indicating success or failure.</returns>
        int LaunchFile(string file, bool debug);
    }

    public interface IProjectLauncher2 : IProjectLauncher {
        /// <summary>
        /// Starts a file in a project with custom settings.
        /// </summary>
        int LaunchFile(string file, bool debug, IProjectLaunchProperties properties);
    }
}
