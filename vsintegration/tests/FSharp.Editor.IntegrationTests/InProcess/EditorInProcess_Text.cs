// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FSharp.Editor.IntegrationTests;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class EditorInProcess
{
    public async Task WaitForCurrentLineTextAsync(string text, CancellationToken cancellationToken)
    {
        await Helper.RetryAsync(async ct =>
        {
            var view = await GetActiveTextViewAsync(cancellationToken);
            var caret = view.Caret.Position.BufferPosition;
            var line = view.TextBuffer.CurrentSnapshot.GetLineFromPosition(caret).GetText();

            return line.Trim() == text.Trim();
        },
            TimeSpan.FromMilliseconds(50),
            cancellationToken);
    }
}