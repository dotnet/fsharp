// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Editor.IntegrationTests.Extensions;
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

    public async Task SetTextAsync(string text, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var textSnapshot = view.TextSnapshot;
        var replacementSpan = new SnapshotSpan(textSnapshot, 0, textSnapshot.Length);
        view.TextBuffer.Replace(replacementSpan, text);
    }

    // count occurrences from 1
    public Task PlaceCaretAsync(string marker, int occurrence, CancellationToken cancellationToken)
        => PlaceCaretAsync(marker, charsOffset: -1, occurrence: occurrence - 1, extendSelection: false, selectBlock: false, cancellationToken);


    private async Task PlaceCaretAsync(
        string marker,
        int charsOffset,
        int occurrence,
        bool extendSelection,
        bool selectBlock,
        CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        dte.Find.FindWhat = marker;
        dte.Find.MatchCase = true;
        dte.Find.MatchInHiddenText = true;
        dte.Find.Target = EnvDTE.vsFindTarget.vsFindTargetCurrentDocument;
        dte.Find.Action = EnvDTE.vsFindAction.vsFindActionFind;

        var originalPosition = await GetCaretPositionAsync(cancellationToken);
        view.Caret.MoveTo(new SnapshotPoint(view.GetBufferContainingCaret()!.CurrentSnapshot, 0));

        if (occurrence > 0)
        {
            var result = EnvDTE.vsFindResult.vsFindResultNotFound;
            for (var i = 0; i < occurrence; i++)
            {
                result = dte.Find.Execute();
            }

            if (result != EnvDTE.vsFindResult.vsFindResultFound)
            {
                throw new Exception("Occurrence " + occurrence + " of marker '" + marker + "' not found in text: " + view.TextSnapshot.GetText());
            }
        }
        else
        {
            var result = dte.Find.Execute();
            if (result != EnvDTE.vsFindResult.vsFindResultFound)
            {
                throw new Exception("Marker '" + marker + "' not found in text: " + view.TextSnapshot.GetText());
            }
        }

        if (charsOffset > 0)
        {
            for (var i = 0; i < charsOffset - 1; i++)
            {
                view.Caret.MoveToNextCaretPosition();
            }

            view.Selection.Clear();
        }

        if (charsOffset < 0)
        {
            // On the first negative charsOffset, move to anchor-point position, as if the user hit the LEFT key
            view.Caret.MoveTo(new SnapshotPoint(view.TextSnapshot, view.Selection.AnchorPoint.Position.Position));

            for (var i = 0; i < -charsOffset - 1; i++)
            {
                view.Caret.MoveToPreviousCaretPosition();
            }

            view.Selection.Clear();
        }

        if (extendSelection)
        {
            var newPosition = view.Selection.ActivePoint.Position.Position;
            view.Selection.Select(new VirtualSnapshotPoint(view.TextSnapshot, originalPosition), new VirtualSnapshotPoint(view.TextSnapshot, newPosition));
            view.Selection.Mode = selectBlock ? TextSelectionMode.Box : TextSelectionMode.Stream;
        }
    }

    public async Task<int> GetCaretPositionAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);

        var subjectBuffer = view.GetBufferContainingCaret();
        Assumes.Present(subjectBuffer);

        var bufferPosition = view.Caret.Position.BufferPosition;
        return bufferPosition.Position;
    }
}
