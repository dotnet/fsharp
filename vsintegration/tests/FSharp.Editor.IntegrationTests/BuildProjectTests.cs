// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class BuildProjectTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task SuccessfulBuild()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var code = """
module Test

let answer = 42
""";

        var expectedBuildSummary = "========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========";

        await solutionExplorer.CreateSolutionAsync(nameof(BuildProjectTests), token);
        await solutionExplorer.AddProjectAsync("Library", template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await editor.SetTextAsync(code, token);

        var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(token);
        
        Assert.Contains(expectedBuildSummary, actualBuildSummary);
    }

    [IdeFact]
    public async Task FailedBuild()
    {
        var token = HangMitigatingCancellationToken;
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var errorList = TestServices.ErrorList;
        var code = """
module Test

let answer =
""";

        var expectedBuildSummary = "========== Build: 0 succeeded, 1 failed, 0 up-to-date, 0 skipped ==========";
        var expectedError = "(Compiler) Library.fs(3, 1): error FS0010: Incomplete structured construct at or before this point in binding";

        await solutionExplorer.CreateSolutionAsync(nameof(BuildProjectTests), token);
        await solutionExplorer.AddProjectAsync("Library", template, token);
        await solutionExplorer.RestoreNuGetPackagesAsync(token);
        await editor.SetTextAsync(code, token);

        var actualBuildSummary = await solutionExplorer.BuildSolutionAsync(token);
        Assert.Contains(expectedBuildSummary, actualBuildSummary);

        await errorList.ShowBuildErrorsAsync(token);
        var errors = await errorList.GetBuildErrorsAsync(__VSERRORCATEGORY.EC_ERROR, token);
        Assert.Contains(expectedError, errors);
    }
}