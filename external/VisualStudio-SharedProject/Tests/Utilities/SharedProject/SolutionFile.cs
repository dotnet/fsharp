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
using System.IO;
using System.Text;
using Microsoft.VisualStudioTools;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Represents a generic solution which can be generated for shared project tests based upon
    /// the language which is being tested.
    /// 
    /// Call Solution.Generate to write the solution out to disk and return an IDisposable object
    /// which when disposed will clean up the solution.
    /// 
    /// You can also get a SolutionFile by calling ProjectDefinition.Generate which will create
    /// a single project SolutionFile.
    /// </summary>
    public sealed class SolutionFile : IDisposable {
        public readonly string Filename;
        public readonly ISolutionElement[] Projects;

        private SolutionFile(string slnFilename, ISolutionElement[] projects) {
            Filename = slnFilename;
            Projects = projects;
        }

        public static SolutionFile Generate(string solutionName, params ISolutionElement[] toGenerate) {
            return Generate(solutionName, -1, toGenerate);
        }

        /// <summary>
        /// Generates the solution file with the specified amount of space remaining relative
        /// to MAX_PATH.
        /// </summary>
        /// <param name="solutionName">The solution name to be created</param>
        /// <param name="pathSpaceRemaining">The amount of path space remaining, or -1 to generate normally</param>
        /// <param name="toGenerate">The projects to be incldued in the generated solution</param>
        /// <returns></returns>
        public static SolutionFile Generate(string solutionName, int pathSpaceRemaining, params ISolutionElement[] toGenerate) {
            List<MSBuild.Project> projects = new List<MSBuild.Project>();
            var location = TestData.GetTempPath(randomSubPath: true);

            if (pathSpaceRemaining >= 0) {
                int targetPathLength = 260 - pathSpaceRemaining;
                location = location + new string('X', targetPathLength - location.Length);
            }
            System.IO.Directory.CreateDirectory(location);

            MSBuild.ProjectCollection collection = new MSBuild.ProjectCollection();
            foreach (var project in toGenerate) {
                projects.Add(project.Save(collection, location));
            }

#if DEV10
            StringBuilder slnFile = new StringBuilder("\r\nMicrosoft Visual Studio Solution File, Format Version 11.00\r\n\u0023 Visual Studio 2010\r\n");
#elif DEV11
            StringBuilder slnFile = new StringBuilder("\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n\u0023 Visual Studio 2012\r\n");
#elif DEV12
            StringBuilder slnFile = new StringBuilder("\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n\u0023 Visual Studio 2013\r\nVisualStudioVersion = 12.0.20827.3\r\nMinimumVisualStudioVersion = 10.0.40219.1\r\n");
#elif DEV14
            StringBuilder slnFile = new StringBuilder("\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n\u0023 Visual Studio 2015\r\nVisualStudioVersion = 14.0.22230.0\r\nMinimumVisualStudioVersion = 10.0.40219.1\r\n");
#else
#error Unsupported VS version
#endif
            for (int i = 0; i < projects.Count; i++) {
                if (toGenerate[i].Flags.HasFlag(SolutionElementFlags.ExcludeFromSolution)) {
                    continue;
                }

                var project = projects[i];
                var projectTypeGuid = toGenerate[i].TypeGuid;

                slnFile.AppendFormat(@"Project(""{0:B}"") = ""{1}"", ""{2}"", ""{3:B}""
EndProject
", projectTypeGuid,
 project != null ? Path.GetFileNameWithoutExtension(project.FullPath) : toGenerate[i].Name,
 project != null ? CommonUtils.GetRelativeFilePath(location, project.FullPath): toGenerate[i].Name,
 project != null ? Guid.Parse(project.GetProperty("ProjectGuid").EvaluatedValue) : Guid.NewGuid());
            }
            slnFile.Append(@"Global
    GlobalSection(SolutionConfigurationPlatforms) = preSolution
        Debug|Any CPU = Debug|Any CPU
        Release|Any CPU = Release|Any CPU
    EndGlobalSection
    GlobalSection(ProjectConfigurationPlatforms) = postSolution
");
            for (int i = 0; i < projects.Count; i++) {
                if (toGenerate[i].Flags.HasFlag(SolutionElementFlags.ExcludeFromConfiguration)) {
                    continue;
                }

                var project = projects[i];
                slnFile.AppendFormat(@"		{0:B}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {0:B}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {0:B}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {0:B}.Release|Any CPU.Build.0 = Release|Any CPU
", Guid.Parse(project.GetProperty("ProjectGuid").EvaluatedValue));
            }

            slnFile.Append(@"	EndGlobalSection
    GlobalSection(SolutionProperties) = preSolution
        HideSolutionNode = FALSE
    EndGlobalSection
EndGlobal
");

            collection.UnloadAllProjects();
            collection.Dispose();

            var slnFilename = Path.Combine(location, solutionName + ".sln");
            File.WriteAllText(slnFilename, slnFile.ToString(), Encoding.UTF8);
            return new SolutionFile(slnFilename, toGenerate);
        }

        public string Directory {
            get {
                return Path.GetDirectoryName(Filename);
            }
        }

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
 