// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Testing;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.VisualStudio.VSConstants;

namespace FSharp.Editor.IntegrationTests;

public class GoToDefinitionTests : AbstractIntegrationTest
{
    // GoToDefn here is subject to several "not ready yet" conditions that a single fixed delay can't
    // reliably bridge:
    //
    //   1. Editor.SetTextAsync / OpenFileAsync writes to the active text buffer synchronously, but Roslyn's
    //      Workspace.CurrentSolution is updated asynchronously by a background propagation step.
    //   2. The F# editor does NOT participate in Roslyn's IAsynchronousOperationListener pattern (grep
    //      vsintegration/src for IAsynchronousOperationListener / BeginAsyncOperation: no matches). It runs
    //      its own cancellableTask-based background typecheck/cache work that is invisible to Roslyn's
    //      "block until all async operations settle" primitive (AsynchronousOperationListenerProvider).
    //      WaitForProjectSystemAsync only covers VS project-system loading, not these hops.
    //   3. The navigation itself is asynchronous: FSharpNavigation.NavigateToItem schedules the actual
    //      caret move as UI work via JoinableTaskFactory, so the caret has NOT moved yet by the time
    //      Shell.ExecuteCommandAsync(GotoDefn) returns. Sampling the caret immediately would misread the
    //      pre-navigation line as a no-op.
    //
    // So we drive GoToDefn off its own observable outcome: anchor the caret on the call site, invoke the
    // command, then POLL for the caret line to change (giving the async navigation time to land). If it
    // never changes within the poll window the checker wasn't ready yet, so we retry the whole command.
    // Crucially we re-anchor the caret only at the START of each attempt -- never between invoking the
    // command and observing its result -- otherwise we would race the async navigation and yank the caret
    // back, making it bounce between the call site and the definition without ever being detected.
    //
    // DO NOT collapse this into a single invocation + one longer wait. That was tried (commit "Replace
    // GoToDefinition retry loop with a single invoke + wait for navigation") and regressed GoesToDefinition
    // on CI with "GoToDefn did not navigate away from 'add 1' within 10s": the FIRST invocation genuinely
    // no-ops (the Roslyn Document / F# checker is not ready for the freshly rebuilt+reopened file yet) and
    // returns without navigating, so no amount of waiting on that one call helps -- the command must be
    // RE-ISSUED. Re-issuing (not waiting longer) is the load-bearing part.
    private const int GoToDefinitionRetryAttempts = 20;
    private static readonly TimeSpan GoToDefinitionRetryDelay = TimeSpan.FromMilliseconds(250);
    private const int GoToDefinitionNavigationPollAttempts = 20;
    private static readonly TimeSpan GoToDefinitionNavigationPollDelay = TimeSpan.FromMilliseconds(100);

    private async Task GoToDefinitionWithRetryAsync(string caretMarker, CancellationToken cancellationToken)
    {
        // First make sure the project system has finished loading the project into the workspace, otherwise
        // the GotoDefn command isn't even routable to the F# editor (it comes back disabled / E_FAIL).
        await Workspace.WaitForProjectSystemAsync(cancellationToken);

        Exception? lastException = null;
        string? lastLine = null;
        for (var attempt = 0; attempt < GoToDefinitionRetryAttempts; attempt++)
        {
            // Re-activate the editor so it is the active command context (PlaceCaretAsync's dte.Find moves
            // the active selection off the editor) and anchor the caret on the call site.
            await Editor.ActivateAsync(cancellationToken);
            await Editor.PlaceCaretAsync(caretMarker, cancellationToken);

            var before = await Editor.GetCurrentLineTextAsync(cancellationToken);
            lastLine = before;

            try
            {
                await Shell.ExecuteCommandAsync(VSStd97CmdID.GotoDefn, cancellationToken);
            }
            catch (COMException ex)
            {
                // The GotoDefn command isn't routable yet (workspace/checker still catching up). Retry.
                lastException = ex;
                await Task.Delay(GoToDefinitionRetryDelay, cancellationToken);
                continue;
            }

            // Poll for the asynchronous navigation to land before treating this attempt as a no-op.
            for (var poll = 0; poll < GoToDefinitionNavigationPollAttempts; poll++)
            {
                var after = await Editor.GetCurrentLineTextAsync(cancellationToken);
                lastLine = after;
                if (!string.Equals(before, after, StringComparison.Ordinal))
                {
                    return;
                }

                await Task.Delay(GoToDefinitionNavigationPollDelay, cancellationToken);
            }

            // Still on the call site after polling: the checker likely hadn't resolved the symbol yet.
            await Task.Delay(GoToDefinitionRetryDelay, cancellationToken);
        }

        throw new InvalidOperationException(
            $"GoToDefn never navigated away from '{caretMarker}' after {GoToDefinitionRetryAttempts} attempts " +
            $"(last line was '{lastLine}'). Last GoToDefn command failure: {lastException?.Message ?? "none"}.");
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
        await GoToDefinitionWithRetryAsync("add 1", TestToken);
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
        await GoToDefinitionWithRetryAsync("SomeType ->", TestToken);
        var expectedText = "type SomeType =";
        var expectedWindow = "Module.fsi";
        var actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        var actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);

        await SolutionExplorer.OpenFileAsync("Library", "Module.fs", TestToken);
        await Editor.PlaceCaretAsync("SomeType)", TestToken);
        await GoToDefinitionWithRetryAsync("SomeType)", TestToken);
        expectedText = "type SomeType =";
        expectedWindow = "Module.fs";
        actualText = await Editor.GetCurrentLineTextAsync(TestToken);
        actualWindow = await Shell.GetActiveWindowCaptionAsync(TestToken);
        Assert.Equal(expectedText, actualText);
        Assert.Equal(expectedWindow, actualWindow);
    }
}