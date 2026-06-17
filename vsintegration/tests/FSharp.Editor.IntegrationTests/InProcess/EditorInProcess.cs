// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Editor.IntegrationTests.Extensions;
using FSharp.Editor.IntegrationTests.Helpers;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class EditorInProcess
{
    // VSStd14CmdID command-set GUID + ShowQuickFixes (Ctrl+.) id; OLECMDEXECOPT_DONTPROMPTUSER = 2.
    private static readonly Guid s_vsStd14CmdSet = new Guid("4c7763bf-5faf-4264-a366-b7e1f27ba958");
    private const uint ShowQuickFixesCmdId = 1;
    private const uint OleCmdExecOptDontPromptUser = 2;

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
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var componentModel = await GetRequiredGlobalServiceAsync<SComponentModel, IComponentModel>(cancellationToken);
        var broker = componentModel.GetService<ILightBulbBroker>();
        var shell = await GetRequiredGlobalServiceAsync<SVsUIShell, IVsUIShell>(cancellationToken);

        // PlaceCaretAsync leaves the active command target on the Find feature, so re-activate the document or the
        // posted ShowQuickFixes command routes to the wrong target.
        await ActivateAsync(cancellationToken);

        // Best-effort deterministic wait for the analyzer/diagnostic work that produces the fixes (focus-independent).
        await AsyncOperationWaiter.WaitForFeaturesAsync(
            componentModel,
            new[] { AsyncOperationWaiter.Workspace, AsyncOperationWaiter.SolutionCrawlerLegacy, AsyncOperationWaiter.DiagnosticService },
            cancellationToken);

        // Invoke the editor's real lightbulb (Ctrl+. / VSStd14CmdID.ShowQuickFixes) so we read the editor-owned
        // session, which is not superseded the way a broker.CreateSession session is. Caller is on the main thread.
        Task ShowLightBulbAsync()
        {
            var cmdGroup = s_vsStd14CmdSet;
            object? inArg = null;
            shell.PostExecCommand(ref cmdGroup, ShowQuickFixesCmdId, OleCmdExecOptDontPromptUser, ref inArg);
            return Task.CompletedTask;
        }

        Task DrainLightBulbAsync(CancellationToken token)
            => AsyncOperationWaiter.WaitForFeaturesAsync(componentModel, new[] { AsyncOperationWaiter.LightBulb }, token);

        try
        {
            return await LightBulbHelper.GetCodeActionsAsync(broker, view, JoinableTaskFactory, ShowLightBulbAsync, DrainLightBulbAsync, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            // Report the error-list contents and tracking state so a failure is self-explaining.
            var entries = await TestServices.ErrorList.GetAllEntriesAsync(cancellationToken);
            throw new InvalidOperationException(
                $"{ex.Message}{Environment.NewLine}trackingEnabled={AsyncOperationWaiter.IsTrackingEnabled()}{Environment.NewLine}" +
                $"--- Error List ({entries.Length} entries) ---{Environment.NewLine}" +
                string.Join(Environment.NewLine, entries),
                ex);
        }
    }
}
