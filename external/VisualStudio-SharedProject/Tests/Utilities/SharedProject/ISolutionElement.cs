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
using System.Threading.Tasks;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Represents a solution element such as a project or solution folder.
    /// </summary>
    public interface ISolutionElement {
        /// <summary>
        /// Gets the name of the solution element
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type guid for the project type or other solution element type such as a folder.
        /// </summary>
        Guid TypeGuid {
            get;
        }

        /// <summary>
        /// Gets the flags which control how the solution element is written to the
        /// solution file.
        /// </summary>
        SolutionElementFlags Flags {
            get;
        }

        /// <summary>
        /// Saves the solution element to disk at the specified location.  The
        /// impelementor can return the created project or null if the solution
        /// element doesn't create a project.
        /// </summary>
        MSBuild.Project Save(MSBuild.ProjectCollection collection, string location);
    }
}
