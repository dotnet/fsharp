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
    /// Generates a file and project item of type Content and if not excluded 
    /// adds it to the generated project.
    /// </summary>
    public sealed class ContentItem : ProjectContentGenerator {
        public readonly string Name;
        public readonly string Content;
        public readonly bool IsExcluded;

        /// <summary>
        /// Creates a new content item with the specifed name and content.
        /// 
        /// If the item is excluded the file will be created, but not added
        /// to the project.
        /// </summary>
        public ContentItem(string name, string content, bool isExcluded = false) {
            Name = name;
            Content = content;
            IsExcluded = isExcluded;
        }

        public override void Generate(ProjectType projectType, MSBuild.Project project) {
            var filename = Path.Combine(project.DirectoryPath, Name);
            File.WriteAllText(filename, Content);

            if (!IsExcluded) {
                project.AddItem("Content", Name);
            }
        }
    }

}
