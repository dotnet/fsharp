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
    /// Defines an interface for providing launch parameters.
    /// </summary>
    public interface IProjectLaunchProperties {
        /// <summary>
        /// Gets the arguments to launch the project with.
        /// </summary>
        string GetArguments();

        /// <summary>
        /// Gets the directory to launch the project in.
        /// </summary>
        string GetWorkingDirectory();

        /// <summary>
        /// Gets the environment variables to set.
        /// </summary>
        /// <param name="includeSearchPaths">
        /// True to also set search path variables.
        /// </param>
        IDictionary<string, string> GetEnvironment(bool includeSearchPaths);
    }
}
