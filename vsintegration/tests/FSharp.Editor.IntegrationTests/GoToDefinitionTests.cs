// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.VisualStudio.VSConstants;

namespace FSharp.Editor.IntegrationTests;

public class GoToDefinitionTests : AbstractIntegrationTest
{
    // GoToDefn resolves at invocation time: if the F# checker has not produced a result for the active
    // document yet, the command no-ops, and only re-issuing it (a retry) or getting lucky with a sleep
    // recovers -- either of which is brittle or masks a real regression. We avoid that by giving every test
    // the setup the F# checker handles cleanly on the first try: the code is written to disk, the project is
    // built, and the file is opened FRESH (never an already-open, buffer-edited document). With that a
    // single GotoDefn resolves, and the only thing left to wait for is the navigation itself --
    // FSharpNavigation.NavigateToItem schedules the caret move asynchronously (via JoinableTaskFactory), so
    // the caret has NOT moved by the time Shell.ExecuteCommandAsync(GotoDefn) returns.
    //
    // CI data behind this: with SetTextAsync on the auto-opened buffer, GoesToDefinition's first invocation
    // deterministically no-ops even after a 10s settle (build 1463623), whereas FsiAndFs -- which adds files
    // to disk and opens them fresh -- passes single-invoke. Editing an already-open buffer is the difference.
    private static readonly TimeSpan GoToDefinitionNavigationTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan GoToDefinitionNavigationPollDelay = TimeSpan.FromMilliseconds(100);

    private async Task GoToDefinitionAsync(string caretMarker, CancellationToken cancellationToken)
    {
        // Wait for the project system to load the project into the workspace.
        await Workspace.WaitForProjectSystemAsync(cancellationToken);

        // Make the editor the active command context (PlaceCaretAsync's dte.Find moves VS's active selection
        // off the editor) and anchor the caret on the call site.
        await Editor.ActivateAsync(cancellationToken);
        await Editor.PlaceCaretAsync(caretMarker, cancellationToken);

        var before = await Editor.GetCurrentLineTextAsync(cancellationToken);
        await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, cancellationToken);

        // Wait for the single asynchronous navigation to move the caret off the call site. This is not a
        // retry of the command -- it just observes the completion of the one navigation we triggered, and
        // fails the test if it never happens (so a real regression is caught).
        for (var elapsed = TimeSpan.Zero; elapsed < GoToDefinitionNavigationTimeout; elapsed += GoToDefinitionNavigationPollDelay)
        {
            var after = await Editor.GetCurrentLineTextAsync(cancellationToken);
            if (!string.Equals(before, after, StringComparison.Ordinal))
            {
                return;
            }

            await Task.Delay(GoToDefinitionNavigationPollDelay, cancellationToken);
        }

        throw new InvalidOperationException(
            $"GoToDefn did not navigate away from '{caretMarker}' within {GoToDefinitionNavigationTimeout.TotalSeconds:0}s.");
    }

    [IdeFact]
    public async Task GoesToDefinition()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var code = """
module Test

let add x y = x + y

let increment = add 1
""";
        var expectedText = "let add x y = x + y";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);
        // Add the code as a real file on disk and open it fresh after building (rather than editing the
        // auto-opened buffer with SetTextAsync), so the F# checker resolves on the first GotoDefn -- see the
        // note on GoToDefinitionAsync. Mirrors FsiAndFsFilesGoToCorrespondentDefinitions.
        await SolutionExplorer.AddFileAsync("Library", "Test.fs", code, TestToken);
        await SolutionExplorer.BuildSolutionAsync(TestToken);
        await SolutionExplorer.OpenFileAsync("Library", "Test.fs", TestToken);

        await GoToDefinitionAsync("add 1", TestToken);
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        
        Assert.Contains(expectedText, actualText);
    }

    [IdeFact]
    public async Task FsiAndFsFilesGoToCorrespondentDefinitions()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        var fsi = """
module Module

type SomeType =
| Number of int
| Letter of char

val id: t: SomeType -> SomeType
""";
        var fs = """
module Module

type SomeType =
    | Number of int
    | Letter of char

let id (t: SomeType) = t
""";

        await SolutionExplorer.CreateSingleProjectSolutionAsync("Library", template, TestToken);
        await SolutionExplorer.RestoreNuGetPackagesAsync(TestToken);

        // hack: when asked to add a file, VS API seems to insert it in the alphabetical order
        // so adding Module.fsi and Module.fs we'll end up having signature file below the code file
        // and this won't work. But it's possible to achieve having the right file order via their renaming
        await SolutionExplorer.AddFileAsync("Library", "AModule.fsi", fsi, TestToken);
        await SolutionExplorer.AddFileAsync("Library", "Module.fs", fs, TestToken);
        await SolutionExplorer.RenameFileAsync("Library", "AModule.fsi", "Module.fsi", TestToken);
        await SolutionExplorer.BuildSolutionAsync(TestToken);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fsi", TestToken);
        await GoToDefinitionAsync("SomeType ->", TestToken);
        var expectedText = "type SomeType =";
        var expectedWindow = "Module.fsi";
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        var actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fs", TestToken);
        await GoToDefinitionAsync("SomeType)", TestToken);
        expectedText = "type SomeType =";
        expectedWindow = "Module.fs";
        actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);
    }
}
