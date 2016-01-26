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

using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Base class for all generated project items.  Override Generate to create
    /// the item on disk (relative to the MSBuild.Project) and optionally add the
    /// generated item to the project.  
    /// </summary>
    public abstract class ProjectContentGenerator {
        /// <summary>
        /// Generates the specified item.  The item can use the project type to 
        /// customize the item.  The item can write it's self out to disk if 
        /// necessary and update the project file appropriately.
        /// </summary>
        public abstract void Generate(ProjectType projectType, MSBuild.Project project);
    }
}
