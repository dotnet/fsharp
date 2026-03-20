// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Extensibility.Testing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

[IdeSettings(MinVersion = VisualStudioVersion.VS18, MaxVersion = VisualStudioVersion.VS18, RootSuffix = "FSharpDebugSeqPoint")]
public class DebuggingSequencePointTests : AbstractIntegrationTest
{
    private const string ProjectName = "DebuggingScenarios";
    private static readonly TimeSpan DebuggerTimeout = TimeSpan.FromSeconds(30);

    /// Regression gate covering issues 12052, 19248, 19255, 13504.
    [IdeFact]
    public async Task BreakpointsHitAcrossReleaseNoteScenarios()
    {
        await PrepareProjectAndBuildAsync();
        await Shell.ClearAllBreakpointsAsync(TestToken);

        var bp12052MatchBranch = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_12052_MATCH_BRANCH");
        var bp12052AfterMatch = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_12052_AFTER_MATCH");
        var bp19248ReturnExpr = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_19248_RETURN_EXPR");
        var bp19255UseBinding = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_19255_USE_BINDING");
        var bp13504BodyLet = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_13504_BODY_LET");

        try
        {
            await Shell.StartDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bp12052MatchBranch, "BP_12052_MATCH_BRANCH");
            await Shell.ContinueDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bp12052AfterMatch, "BP_12052_AFTER_MATCH");
            await Shell.ContinueDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bp19248ReturnExpr, "BP_19248_RETURN_EXPR");
            await Shell.ContinueDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bp19255UseBinding, "BP_19255_USE_BINDING");
            await Shell.ContinueDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bp13504BodyLet, "BP_13504_BODY_LET");
            await Shell.ContinueDebuggingAsync(TestToken);
            await Shell.WaitForDesignModeAsync(DebuggerTimeout, TestToken);
        }
        finally
        {
            await Shell.StopDebuggingAsync(TestToken);
            await Shell.ClearAllBreakpointsAsync(TestToken);
        }
    }

    /// Tests hot-binding a breakpoint while the debugger is already running.
    [IdeFact]
    public async Task Issue13504_ListComprehensionBody()
    {
        await PrepareProjectAndBuildAsync();
        await Shell.ClearAllBreakpointsAsync(TestToken);
        var anchorLine = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_12052_MATCH_BRANCH");

        try
        {
            await Shell.StartDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(anchorLine, "BP_12052_MATCH_BRANCH");
            var bodyLine = await ToggleBreakpointAtMarkerAndGetLineAsync("BP_13504_BODY_LET");
            await Shell.ContinueDebuggingAsync(TestToken);
            await AssertBreakpointHitAsync(bodyLine, "BP_13504_BODY_LET");
            await Shell.ContinueDebuggingAsync(TestToken);
            await Shell.WaitForDesignModeAsync(DebuggerTimeout, TestToken);
        }
        finally
        {
            await Shell.StopDebuggingAsync(TestToken);
            await Shell.ClearAllBreakpointsAsync(TestToken);
        }
    }

    private async Task PrepareProjectAndBuildAsync()
    {
        await SolutionExplorer.CreateSingleProjectSolutionAsync(ProjectName, SolutionExplorerInProcess.ExistingProjectTemplate, TestToken);
        await SolutionExplorer.SetStartupProjectAsync(ProjectName, TestToken);

        await SolutionExplorer.OpenFileAsync(ProjectName, "Program.fs", TestToken);
        await Editor.SetTextAsync(File.ReadAllText(GetFixturePath()), TestToken);
        await Shell.ExecuteCommandAsync("File.SaveAll", TestToken);

        var buildSummary = await SolutionExplorer.BuildSolutionAsync(TestToken);
        Assert.NotNull(buildSummary);
        Assert.Contains("Build: 1 succeeded, 0 failed", string.Join(Environment.NewLine, buildSummary));
    }

    private static string GetFixturePath()
    {
        var dir = Path.GetDirectoryName(typeof(DebuggingSequencePointTests).Assembly.Location)!;
        return Path.Combine(dir, "TestData", "Debugging", "SequencePointIssues.fs");
    }

    private async Task<int> ToggleBreakpointAtMarkerAndGetLineAsync(string marker)
    {
        var before = (await Shell.GetConfiguredBreakpointLinesAsync(TestToken)).ToList();
        await Editor.ToggleBreakpointAtMarkerAsync(marker, TestToken);
        var after = (await Shell.GetConfiguredBreakpointLinesAsync(TestToken)).ToList();
        foreach (var line in before) { var i = after.IndexOf(line); if (i >= 0) after.RemoveAt(i); }
        return Assert.Single(after);
    }

    private async Task AssertBreakpointHitAsync(int expectedLine, string label)
    {
        await Shell.WaitForBreakpointHitAsync(DebuggerTimeout, continueOnStepBreak: true, TestToken);
        var actual = await Shell.GetLastHitBreakpointLineAsync(TestToken);
        Assert.True(actual == expectedLine, $"{label}: expected line {expectedLine}, hit {actual}");
    }
}
