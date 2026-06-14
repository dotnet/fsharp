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
    // GoToDefn only resolves once the editor's backing Roslyn Document and the F# checker are ready for the
    // file being navigated:
    //
    //   * Editor.SetTextAsync / OpenFileAsync update the buffer synchronously, but Roslyn propagates that
    //     edit to Workspace.CurrentSolution asynchronously, and the F# editor then typechecks on its own
    //     cancellableTask infrastructure. F# does NOT participate in Roslyn's IAsynchronousOperationListener
    //     pattern (grep vsintegration/src for IAsynchronousOperationListener / BeginAsyncOperation: no
    //     matches), so AsynchronousOperationListenerProvider.WaitAllAsync cannot observe it and
    //     WaitForProjectSystemAsync only covers VS project-system load. If GotoDefn fires before that
    //     settles, the F# checker resolves nothing and the command no-ops.
    //   * The navigation itself is asynchronous: FSharpNavigation.NavigateToItem schedules the caret move as
    //     UI work via JoinableTaskFactory, so the caret has NOT moved by the time
    //     Shell.ExecuteCommandAsync(GotoDefn) returns.
    //
    // We deliberately issue GotoDefn ONCE rather than retrying: a retry loop would mask a genuine
    // GoToDefinition regression (one where the first invocations stop navigating) by eventually succeeding.
    // Instead we wait for the checker to settle after the file has been opened/built, issue the command
    // once, and then wait for that single navigation to land.
    private static readonly TimeSpan FSharpCheckerSettleDelay = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan GoToDefinitionNavigationTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan GoToDefinitionNavigationPollDelay = TimeSpan.FromMilliseconds(100);

    private async Task GoToDefinitionAsync(string caretMarker, CancellationToken cancellationToken)
    {
        // Wait for the project system to load the project into the workspace, then give the F# checker time
        // to typecheck the active (just opened/built) document so the single GotoDefn below can resolve.
        await Workspace.WaitForProjectSystemAsync(cancellationToken);
        await Task.Delay(FSharpCheckerSettleDelay, cancellationToken);

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
        await Editor.SetTextAsync(code, TestToken);
        // Build so the F# checker has the project's full options; without it GoToDefinition can't resolve
        // the symbol and no-ops. Building leaves the Build Output pane as the active text view, so re-open
        // the source file afterwards to make it the active document again (mirrors how
        // FsiAndFsFilesGoToCorrespondentDefinitions builds and then opens the file it navigates in).
        await SolutionExplorer.BuildSolutionAsync(TestToken);
        await SolutionExplorer.OpenFileAsync("Library", "Library.fs", TestToken);

        await Editor.PlaceCaretAsync("add 1", TestToken);
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
        await Editor.PlaceCaretAsync("SomeType ->", TestToken);
        await GoToDefinitionAsync("SomeType ->", TestToken);
        var expectedText = "type SomeType =";
        var expectedWindow = "Module.fsi";
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        var actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fs", TestToken);
        await Editor.PlaceCaretAsync("SomeType)", TestToken);
        await GoToDefinitionAsync("SomeType)", TestToken);
        expectedText = "type SomeType =";
        expectedWindow = "Module.fs";
        actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);
    }
}
