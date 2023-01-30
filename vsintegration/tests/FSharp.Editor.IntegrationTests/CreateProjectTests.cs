// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace FSharp.Editor.IntegrationTests
{
    public class CreateProjectTests : AbstractIntegrationTest
    {
        [IdeFact]
        public async Task BasicFSharpLibraryCompilesAsync()
        {
            await TestServices.SolutionExplorer.CreateSolutionAsync(
                nameof(CreateProjectTests),
                HangMitigatingCancellationToken);

            await TestServices.SolutionExplorer.AddProjectAsync(
                "Library",
                "Microsoft.FSharp.NETCore.ClassLibrary",
                HangMitigatingCancellationToken);

            await TestServices.SolutionExplorer.RestoreNuGetPackagesAsync(HangMitigatingCancellationToken);

            var buildSummary = await TestServices.SolutionExplorer.BuildSolutionAsync(
                true,
                HangMitigatingCancellationToken);
            Assert.Contains(
                "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========",
                buildSummary);

            await TestServices.ErrorList.ShowBuildErrorsAsync(HangMitigatingCancellationToken);
            var errorCount = await TestServices.ErrorList.GetErrorCountAsync(
                __VSERRORCATEGORY.EC_ERROR,
                HangMitigatingCancellationToken);
            Assert.Equal(0, errorCount);
        }
    }
}