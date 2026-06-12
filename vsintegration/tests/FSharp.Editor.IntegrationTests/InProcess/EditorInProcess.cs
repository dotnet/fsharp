// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Editor.IntegrationTests.Extensions;
using FSharp.Editor.IntegrationTests.Helpers;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class EditorInProcess
{
    public async Task<string> GetTextAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var textSnapshot = view.TextSnapshot;
        return textSnapshot.GetText();
    }

    public async Task<string> GetCurrentLineTextAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await TestServices.Editor.GetActiveTextViewAsync(cancellationToken);
        var bufferPosition = view.Caret.Position.BufferPosition;
        var line = bufferPosition.GetContainingLine();
        return line.GetText();
    }

    public async Task SetTextAsync(string text, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var textSnapshot = view.TextSnapshot;
        var replacementSpan = new SnapshotSpan(textSnapshot, 0, textSnapshot.Length);
        view.TextBuffer.Replace(replacementSpan, text);
    }

    public async Task PlaceCaretAsync(string marker, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        dte.Find.FindWhat = marker;
        dte.Find.MatchCase = true;
        dte.Find.MatchInHiddenText = true;
        dte.Find.Target = EnvDTE.vsFindTarget.vsFindTargetCurrentDocument;
        dte.Find.Action = EnvDTE.vsFindAction.vsFindActionFind;

        view.Caret.MoveTo(new SnapshotPoint(view.GetBufferContainingCaret()!.CurrentSnapshot, 0));

        var result = dte.Find.Execute();
        if (result != EnvDTE.vsFindResult.vsFindResultFound)
        {
            throw new Exception("Marker '" + marker + "' not found in text: " + view.TextSnapshot.GetText());
        }

        // On the first negative charsOffset, move to anchor-point position, as if the user hit the LEFT key
        view.Caret.MoveTo(new SnapshotPoint(view.TextSnapshot, view.Selection.AnchorPoint.Position.Position));

        view.Selection.Clear();
    }

    public async Task<IEnumerable<SuggestedActionSet>> InvokeCodeActionListAsync(CancellationToken cancellationToken)
    {
        // Poll-and-retry: the lightbulb session can race with F# diagnostics computation in two ways:
        //   - broker.GetSession(view) returns null when no session is active yet (NRE on cast).
        //   - The session opens but self-dismisses before reaching Completed/CompletedWithoutData,
        //     producing TaskCanceledException from LightBulbHelper.WaitForItemsAsync.
        // Each retry posts a fresh ShowQuickFixes command (creating a new session) and tolerates
        // both failure modes. Bounded to ~5 seconds; the outer test ct still caps total wait.
        const int MaxAttempts = 20;
        var attemptDelay = TimeSpan.FromMilliseconds(250);

        var shell = await GetRequiredGlobalServiceAsync<SVsUIShell, IVsUIShell>(cancellationToken);
        var broker = await GetComponentModelServiceAsync<ILightBulbBroker>(cancellationToken);
        var cmdGroup = typeof(VSConstants.VSStd14CmdID).GUID;
        var cmdID = VSConstants.VSStd14CmdID.ShowQuickFixes;

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            object? obj = null;
            shell.PostExecCommand(cmdGroup, (uint)cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref obj);
            var view = await GetActiveTextViewAsync(cancellationToken);

            try
            {
                var lightbulbs = await LightBulbHelper.WaitForItemsAsync(broker, view, cancellationToken);
                if (lightbulbs is not null && System.Linq.Enumerable.Any(lightbulbs))
                {
                    return lightbulbs;
                }
            }
            catch (NullReferenceException) when (attempt < MaxAttempts)
            {
                // broker.GetSession(view) returned null; F# analyzer hasn't surfaced a session yet.
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && attempt < MaxAttempts)
            {
                // Session opened but was dismissed before producing actions; try again.
            }

            await Task.Delay(attemptDelay, cancellationToken);
        }

        // Final attempt: let the underlying exception propagate so the test failure message
        // points at the actual root cause (NRE / TaskCanceled) rather than a vague timeout.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        {
            object? obj = null;
            shell.PostExecCommand(cmdGroup, (uint)cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref obj);
            var view = await GetActiveTextViewAsync(cancellationToken);
            return await LightBulbHelper.WaitForItemsAsync(broker, view, cancellationToken);
        }
    }
}
