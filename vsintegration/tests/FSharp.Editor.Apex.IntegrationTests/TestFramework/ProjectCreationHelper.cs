//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            return this.CreateFSharpProject(ProjectTemplate.ClassLibrary, projectName);
        }

        /// <summary>
        /// Creates a new F# project of the given built-in template with the given name.
        /// </summary>
        public ProjectTestExtension CreateFSharpProject(ProjectTemplate template, string projectName)
            => this.visualStudio.ObjectModel.Solution.CreateProject<ProjectTestExtension>(
                ProjectLanguage.FSharp,
                template,
                projectName);

        /// <summary>
        /// Creates a fresh single-project solution from an F# SDK template (e.g. "classlib", "console",
        /// "xunit") scaffolded on disk with <c>dotnet new</c>. Apex's <see cref="ProjectTemplate"/> enum
        /// maps to the legacy .NET Framework F# templates (which produce Library1.fs/Script.fsx), whereas
        /// the source FSharp.Editor.IntegrationTests target the .NET SDK templates; using <c>dotnet new</c>
        /// makes the project (filenames and default source) match those. An empty solution is created
        /// first because <c>AddProject</c> throws when no solution is open.
        /// </summary>
        public ProjectTestExtension CreateFSharpProjectFromSdkTemplate(string sdkTemplateName, string projectName)
        {
            string projectDirectory = Path.Combine(
                Path.GetTempPath(), "FSharpApexTemplates", Guid.NewGuid().ToString("N"), projectName);
            Directory.CreateDirectory(projectDirectory);

            RunDotNet($"new {sdkTemplateName} --language \"F#\" --name \"{projectName}\" --output \"{projectDirectory}\"");

            string projectPath = Path.Combine(projectDirectory, projectName + ".fsproj");
            this.visualStudio.ObjectModel.Solution.CreateEmptySolution();
            return this.visualStudio.ObjectModel.Solution.AddProject(projectPath);
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

        /// <summary>
        /// Creates a file with the given content on disk and adds it to the given project or item.
        /// </summary>
        public ProjectItemTestExtension AddProjectItemFromContent(IProjectItemsContainer container, string fileName, string content)
        {
            string containerDirectory = container.IsProject ? container.ProjectDirectory : container.FullPath;
            string filePath = Path.Combine(containerDirectory, fileName);
            File.WriteAllText(filePath, content);
            return container.AddProjectItemFromFile(filePath);
        }

        private static void RunDotNet(string arguments)
        {
            var startInfo = new ProcessStartInfo("dotnet", arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"Failed to start 'dotnet {arguments}'.");

            // Drain both redirected streams concurrently. Reading one to completion before the other can
            // deadlock: if the child fills the stdout pipe buffer while we block on stderr's EOF (which
            // only arrives at process exit), neither side can make progress.
            Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
            process.StandardOutput.ReadToEnd();
            string standardError = standardErrorTask.GetAwaiter().GetResult();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"'dotnet {arguments}' failed with exit code {process.ExitCode}: {standardError}");
            }
        }
    }
}
