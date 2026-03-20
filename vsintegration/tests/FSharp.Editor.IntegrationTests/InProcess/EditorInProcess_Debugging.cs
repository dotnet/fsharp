// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class EditorInProcess
{
    public async Task ToggleBreakpointAtMarkerAsync(string marker, CancellationToken cancellationToken)
    {
        await PlaceCaretAsync(marker, cancellationToken);
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var view = await GetActiveTextViewAsync(cancellationToken);
        var lineStart = view.Caret.Position.BufferPosition.GetContainingLine().Start;
        view.Caret.MoveTo(lineStart);
        view.Selection.Clear();

        await TestServices.Shell.ExecuteCommandAsync("Debug.ToggleBreakpoint", cancellationToken);
    }
}
