//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.IO;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Solution;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Creates F# projects and project items for the Apex integration tests.
    /// Tailored for the F# extension: the TypeScript-VS ProjectCreationHelper created ASP.NET Core
    /// web apps for JavaScript/TypeScript; this one creates plain F# class libraries.
    /// </summary>
    public sealed class ProjectCreationHelper
    {
        private readonly VisualStudioHost visualStudio;
        private int projectCounter = 0;

        public ProjectCreationHelper(VisualStudioHost visualStudio)
        {
            this.visualStudio = visualStudio;
        }

        /// <summary>
        /// Creates a new F# class library in a fresh solution.
        /// </summary>
        public ProjectTestExtension CreateFSharpLibrary()
        {
            string projectName = $"FSharpLibrary_{++this.projectCounter}";
            return this.visualStudio.ObjectModel.Solution.CreateProject<ProjectTestExtension>(
                ProjectLanguage.FSharp,
                ProjectTemplate.ClassLibrary,
                projectName);
        }

        /// <summary>
        /// Creates an empty file on disk and adds it to the given project or project item.
        /// </summary>
        public ProjectItemTestExtension AddProjectItemFromEmptyFile(IProjectItemsContainer container, string fileName)
        {
            string containerDirectory = container.IsProject ? container.ProjectDirectory : container.FullPath;
            string filePath = Path.Combine(containerDirectory, fileName);
            File.WriteAllText(filePath, string.Empty);
            return container.AddProjectItemFromFile(filePath);
        }
    }
}
