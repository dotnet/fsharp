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
    /// Generates a custom msbuild item .
    /// 
    /// The item is added to the project if not excluded.
    /// </summary>
    public sealed class CustomItem : ProjectContentGenerator {
        public readonly string Name, Content, ItemType;
        public readonly bool IsExcluded;
        public readonly bool IsMissing;
        public readonly IEnumerable<KeyValuePair<string, string>> Metadata;

        /// <summary>
        /// Creates a new custom item with the specifed type, name, content, and metadata.
        /// </summary>
        public CustomItem(string itemType, string name, string content = null, bool isExcluded = false, bool isMissing = false, IEnumerable<KeyValuePair<string, string>> metadata = null) {
            ItemType = itemType;
            Name = name;
            IsExcluded = isExcluded;
            IsMissing = isMissing;
            Content = content;
            Metadata = metadata;
        }

        public override void Generate(ProjectType projectType, MSBuild.Project project) {
            var filename = Path.Combine(project.DirectoryPath, Name);
            if (!IsMissing) {
                File.WriteAllText(filename, Content);
            }

            if (!IsExcluded) {
                project.AddItem(
                    ItemType,
                    Name,
                    Metadata
                );
            }
        }
    }

}
