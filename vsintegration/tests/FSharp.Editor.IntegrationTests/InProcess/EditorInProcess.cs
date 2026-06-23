// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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

        // Trace document state
        var caretPos = view.Caret.Position.BufferPosition.Position;
        var docText = view.TextSnapshot.GetText();
        System.Diagnostics.Trace.TraceInformation(
            "[EditorInProcess] InvokeCodeActionListAsync: caretPos={0}, docLength={1}, docText=<<<{2}>>>",
            caretPos, docText.Length, docText.Length <= 500 ? docText : docText.Substring(0, 500));

        await ActivateAsync(cancellationToken);

        System.Diagnostics.Trace.TraceInformation("[EditorInProcess] InvokeCodeActionListAsync: waiting for features (Workspace, SolutionCrawlerLegacy, DiagnosticService)");

        await AsyncOperationWaiter.WaitForFeaturesAsync(
            componentModel,
            new[] { AsyncOperationWaiter.Workspace, AsyncOperationWaiter.SolutionCrawlerLegacy, AsyncOperationWaiter.DiagnosticService },
            cancellationToken);

        System.Diagnostics.Trace.TraceInformation("[EditorInProcess] InvokeCodeActionListAsync: features drained, invoking lightbulb");

        Task ShowLightBulbAsync()
        {
            var cmdGroup = s_vsStd14CmdSet;
            object? inArg = null;
            shell.PostExecCommand(ref cmdGroup, ShowQuickFixesCmdId, OleCmdExecOptDontPromptUser, ref inArg);
            return Task.CompletedTask;
        }

        Task DrainLightBulbAsync(CancellationToken token)
            => AsyncOperationWaiter.WaitForFeaturesAsync(componentModel, new[] { AsyncOperationWaiter.LightBulb }, token);

        Task TriggerReanalysisAsync(CancellationToken token)
            => TriggerDiagnosticsAsync(view, token);

        try
        {
            return await LightBulbHelper.GetCodeActionsAsync(broker, view, JoinableTaskFactory, ShowLightBulbAsync, DrainLightBulbAsync, TriggerReanalysisAsync, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            var entries = await TestServices.ErrorList.GetAllEntriesAsync(cancellationToken);
            var categoryRegistry = componentModel.GetService<ISuggestedActionCategoryRegistryService>();
            var probe = await ProbeAvailableActionsAsync(broker, view, categoryRegistry.Any, cancellationToken);
            throw new InvalidOperationException(
                $"{ex.Message}{Environment.NewLine}trackingEnabled={AsyncOperationWaiter.IsTrackingEnabled()}{Environment.NewLine}" +
                $"probe: {probe}{Environment.NewLine}" +
                $"--- Error List ({entries.Length} entries) ---{Environment.NewLine}" +
                string.Join(Environment.NewLine, entries),
                ex);
        }
    }

    // Bumps the document version with a net-zero edit (insert+delete a space at EOF) to force F# to recompute
    // diagnostics on the now fully-loaded project, restoring the caret afterward.
    private async Task TriggerDiagnosticsAsync(IWpfTextView view, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var caret = view.Caret.Position.BufferPosition.Position;
        var buffer = view.TextBuffer;
        buffer.Insert(buffer.CurrentSnapshot.Length, " ");
        buffer.Delete(new Span(buffer.CurrentSnapshot.Length - 1, 1));

        var snapshot = buffer.CurrentSnapshot;
        view.Caret.MoveTo(new SnapshotPoint(snapshot, Math.Min(caret, snapshot.Length)));
    }

    // Asks the lightbulb broker (producer-agnostic) whether any suggested actions exist at the caret and which
    // categories, without creating a session - so a failure tells us if the fix was simply never offered.
    private async Task<string> ProbeAvailableActionsAsync(ILightBulbBroker broker, IWpfTextView view, ISuggestedActionCategorySet requested, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var result = "";
        try
        {
            var has = await broker.HasSuggestedActionsAsync(requested, view, cancellationToken);
            result += $"HasSuggestedActions={has}";
        }
        catch (Exception ex)
        {
            result += $"HasSuggestedActions threw {ex.GetType().Name}: {ex.Message}";
        }

        try
        {
            var categories = await ((ILightBulbBroker2)broker).GetSuggestedActionCategoriesAsync(requested, view, cancellationToken);
            var names = categories is null ? Array.Empty<string>() : ((IEnumerable<string>)categories).ToArray();
            result += $"; categories=[{string.Join(",", names)}]";
        }
        catch (Exception ex)
        {
            result += $"; categories threw {ex.GetType().Name}: {ex.Message}";
        }

        return result;
    }
}
