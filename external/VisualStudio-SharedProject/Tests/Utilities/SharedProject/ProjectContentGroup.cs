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
    /// Groups a set of ProjectContentGenerator together.
    /// 
    /// This class exists solely to allow a hierarchy to be written in
    /// source code when describing the test projects.
    /// 
    /// It takes a list of ProjectContentGenerator, and when asked to
    /// generate will generate the list in order.
    /// </summary>
    public class ProjectContentGroup : ProjectContentGenerator {
        private readonly ProjectContentGenerator[] _content;

        public ProjectContentGroup(ProjectContentGenerator[] content) {
            _content = content;
        }

        public override void Generate(ProjectType projectType, MSBuild.Project project) {
            foreach (var content in _content) {
                content.Generate(projectType, project);
            }
        }
    }
}
