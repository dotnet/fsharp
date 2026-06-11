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
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var shell = await GetRequiredGlobalServiceAsync<SVsUIShell, IVsUIShell>(cancellationToken);
        var cmdGroup = typeof(VSConstants.VSStd14CmdID).GUID;
        var cmdID = VSConstants.VSStd14CmdID.ShowQuickFixes;

        // Post ShowQuickFixes once. PostExecCommand is fire-and-forget; the broker spins up the
        // session asynchronously after F# diagnostics surface, so we have to wait for the session
        // to materialize before LightBulbHelper.WaitForItemsAsync can subscribe to its events.
        // Repeated PostExecCommand calls would dismiss any in-flight session and reset the race.
        object? obj = null;
        shell.PostExecCommand(cmdGroup, (uint)cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref obj);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var broker = await GetComponentModelServiceAsync<ILightBulbBroker>(cancellationToken);

        // Poll for the session to appear (up to ~5 s). Without this wait, the F#-analyzer-driven
        // code fixes (UnusedOpenDeclarations, AddMissingFunKeyword) NRE because broker.GetSession
        // returns null before the lightbulb session is ready -- LightBulbHelper.WaitForItemsAsync
        // then casts null to IAsyncLightBulbSession and subscribes to its events.
        var sessionTimeout = TimeSpan.FromSeconds(5);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (broker.GetSession(view) is null)
        {
            if (sw.Elapsed > sessionTimeout)
            {
                // Final attempt -- let LightBulbHelper's existing NRE propagate so the test failure
                // points at the actual root cause, not a manufactured timeout exception.
                break;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }

        return await LightBulbHelper.WaitForItemsAsync(broker, view, cancellationToken);
    }
}
