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
using System.IO;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Generates a source code file.  The extension will be the code extension for
    /// the project type being generated and content will be the default content.
    /// 
    /// The item is added to the project if not excluded.
    /// </summary>
    public sealed class CompileItem : ProjectContentGenerator {
        public readonly string Name, Content, LinkFile;
        public readonly bool IsExcluded;
        public readonly bool IsMissing;

        /// <summary>
        /// Creates a new compile item.  The item will be generated with the 
        /// projects code file extension and sample code.  If the item is excluded
        /// then the file will be written out but not added to the project.
        /// If the item is missing then the file will not be written to disk.
        /// 
        /// If content is not provided or is null then the default sample code
        /// from the project type will be used.
        /// </summary>
        public CompileItem(string name, string content = null, bool isExcluded = false, bool isMissing = false, string link = null) {
            Name = name;
            IsExcluded = isExcluded;
            IsMissing = isMissing;
            Content = content;
            LinkFile = link;
        }

        public override void Generate(ProjectType projectType, MSBuild.Project project) {
            var filename = Path.Combine(project.DirectoryPath, Name + projectType.CodeExtension);
            if (!IsMissing) {
                File.WriteAllText(filename, Content ?? projectType.SampleCode);
            }

            if (!IsExcluded) {
                List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
                if (LinkFile != null) {
                    metadata.Add(new KeyValuePair<string, string>("Link", LinkFile + projectType.CodeExtension));
                }

                project.AddItem(
                    "Compile",
                    Name + projectType.CodeExtension,
                    metadata
                );
            }
        }

        public CompileItem Link(string link) {
            return new CompileItem(
                Name,
                Content,
                IsExcluded,
                IsMissing,
                link
            );
        }
    }

}
