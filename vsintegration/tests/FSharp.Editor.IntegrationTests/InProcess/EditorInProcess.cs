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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

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

    // dte.Find.Execute (used by PlaceCaretAsync) leaves VS's active selection on the Find feature rather
    // than the editor, so shell commands dispatched afterwards through SUIHostCommandDispatcher (e.g.
    // VSStd97CmdID.GotoDefn) are routed to the wrong command target and come back disabled (E_FAIL).
    // Re-activate the document window so the editor is the active command context again. This uses VS's
    // internal active-frame selection (IVsMonitorSelection) and does not depend on the OS foreground window.
    public async Task ActivateAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        dte.ActiveDocument?.Activate();
    }

    public async Task<IEnumerable<SuggestedActionSet>> InvokeCodeActionListAsync(CancellationToken cancellationToken)
        => await InvokeCodeActionListAsync(waitForErrorListDiagnostics: true, cancellationToken);

    // waitForErrorListDiagnostics: skip for fixes whose diagnostic is Hidden (e.g. F# unused-opens), which
    // never appears in the error list - waiting on it there can never succeed and just wastes the timeout.
    public async Task<IEnumerable<SuggestedActionSet>> InvokeCodeActionListAsync(bool waitForErrorListDiagnostics, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var broker = await GetComponentModelServiceAsync<ILightBulbBroker>(cancellationToken);
        var categoryRegistry = await GetComponentModelServiceAsync<ISuggestedActionCategoryRegistryService>(cancellationToken);

        // Bring back the 2-minute settle (best producer-agnostic result so far): optionally wait quietly for the
        // document's diagnostics (no lightbulb churn), then settle for 2 minutes so lagging fix computations
        // are ready, then invoke. Running this a few times to measure how flaky it really is.
        if (waitForErrorListDiagnostics)
        {
            await WaitForDocumentDiagnosticsAsync(cancellationToken);
        }

        await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);

        try
        {
            return await LightBulbHelper.GetCodeActionsAsync(broker, view, categoryRegistry.Any, JoinableTaskFactory, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            // Report the error-list contents so a failure is self-explaining.
            var entries = await TestServices.ErrorList.GetAllEntriesAsync(cancellationToken);
            throw new InvalidOperationException(
                $"{ex.Message}{Environment.NewLine}--- Error List ({entries.Length} entries) ---{Environment.NewLine}" +
                string.Join(Environment.NewLine, entries),
                ex);
        }
    }

    // Polls the error list (lightly, no lightbulb activity) until the document has at least one diagnostic of
    // any severity, or a bounded timeout elapses. Best-effort: on timeout we still invoke. (Note: F# unused-opens
    // is a Hidden diagnostic and never appears here, so for that test this just acts as extra settle time.)
    private async Task WaitForDocumentDiagnosticsAsync(CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(60);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var count = await TestServices.ErrorList.GetErrorCountAsync(__VSERRORCATEGORY.EC_MESSAGE, cancellationToken);
            if (count > 0)
            {
                return;
            }

            await Task.Delay(500, cancellationToken);
        }
    }
}
