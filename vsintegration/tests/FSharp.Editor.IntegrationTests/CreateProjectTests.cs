// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FSharp.Editor.IntegrationTests
{
    public class CreateProjectTests : AbstractIntegrationTest
    {
        [IdeFact]
        public async Task ClassLibrary_Async()
        {
            var token = HangMitigatingCancellationToken;
            var solutionExplorer = TestServices.SolutionExplorer;
            var errorList = TestServices.ErrorList;

            await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
            await solutionExplorer.AddProjectAsync(
                "Library",
                WellKnownProjectTemplates.FSharpNetCoreClassLibrary,
                token);

            await solutionExplorer.RestoreNuGetPackagesAsync(token);

            var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";
            var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(true, token);
            Assert.Contains(expectedBuildSummary, actualBuildSummary);

            await errorList.ShowBuildErrorsAsync(token);
            var errorCount = await errorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_ERROR, token);
            Assert.Equal(0, errorCount);
        }

        [IdeFact]
        public async Task ConsoleApp_Async()
        {
            var token = HangMitigatingCancellationToken;
            var solutionExplorer = TestServices.SolutionExplorer;
            var errorList = TestServices.ErrorList;

            await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
            await solutionExplorer.AddProjectAsync(
                "ConsoleApp",
                WellKnownProjectTemplates.FSharpNetCoreConsoleApplication,
                token);

            await solutionExplorer.RestoreNuGetPackagesAsync(token);

            var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";
            var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(true, token);
            Assert.Contains(expectedBuildSummary, actualBuildSummary);

            await errorList.ShowBuildErrorsAsync(token);
            var errorCount = await errorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_ERROR, token);
            Assert.Equal(0, errorCount);
        }

        [IdeFact]
        public async Task XUnitTestProject_Async()
        {
            var token = HangMitigatingCancellationToken;
            var solutionExplorer = TestServices.SolutionExplorer;
            var errorList = TestServices.ErrorList;

            await solutionExplorer.CreateSolutionAsync(nameof(CreateProjectTests), token);
            await solutionExplorer.AddProjectAsync(
                "ConsoleApp",
                WellKnownProjectTemplates.FSharpNetCoreXUnitTest,
                token);

            await solutionExplorer.RestoreNuGetPackagesAsync(token);

            var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";
            var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(true, token);
            Assert.Contains(expectedBuildSummary, actualBuildSummary);

            await errorList.ShowBuildErrorsAsync(token);
            var errorCount = await errorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_ERROR, token);
            Assert.Equal(0, errorCount);
        }
    }
}