// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class BuildProjectTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task SuccessfulBuild()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var code = """
module Test

let answer = 42
""";
        var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";

        await SolutionExplorer.CreateSolutionAsync(nameof(BuildProjectTests), TestToken);
        await SolutionExplorer.AddProjectAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);

        var actualBuildSummary = await SolutionExplorer.BuildSolutionAsync(TestToken);

        Assert.Contains(expectedBuildSummary, actualBuildSummary);
    }

    [IdeFact]
    public async Task FailedBuild()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var code = """
module Test

let answer =
""";
        var expectedBuildSummary = "========== Build: 0 succeeded, 1 failed, 0 up-to-date, 0 skipped ==========";
        var expectedError = "(Compiler) Library.fs(3, 1): error FS0010: Incomplete structured construct at or before this point in binding";

        await SolutionExplorer.CreateSolutionAsync(nameof(BuildProjectTests), TestToken);
        await SolutionExplorer.AddProjectAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        await Editor.SetTextAsync(code, TestToken);

        var actualBuildSummary = await SolutionExplorer.BuildSolutionAsync(TestToken);
        Assert.Contains(expectedBuildSummary, actualBuildSummary);

        await ErrorList.ShowBuildErrorsAsync(TestToken);
        var errors = await ErrorList.GetBuildErrorsAsync(__VSERRORCATEGORY.EC_ERROR, TestToken);
        Assert.Contains(expectedError, errors);
    }
}