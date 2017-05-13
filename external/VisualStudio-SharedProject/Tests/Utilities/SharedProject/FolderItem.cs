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

using System.IO;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Generates a folder and if not excluded adds it to the generated project.
    /// </summary>
    public sealed class FolderItem : ProjectContentGenerator {
        public readonly string Name;
        public readonly bool IsExcluded, IsMissing;

        /// <summary>
        /// Creates a new folder with the specified name.  If the folder
        /// is excluded then it will be created on disk but not added to the
        /// project.
        /// </summary>
        public FolderItem(string name, bool isExcluded = false, bool isMissing = false) {
            Name = name;
            IsExcluded = isExcluded;
            IsMissing = isMissing;
        }

        public override void Generate(ProjectType projectType, MSBuild.Project project) {
            if (!IsMissing) {
                Directory.CreateDirectory(Path.Combine(project.DirectoryPath, Name));
            }

            if (!IsExcluded) {
                project.AddItem("Folder", Name);
            }
        }
    }
}
