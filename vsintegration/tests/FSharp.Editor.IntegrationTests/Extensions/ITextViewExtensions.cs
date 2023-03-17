// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace FSharp.Editor.IntegrationTests.Extensions;

internal static class ITextViewExtensions
{
    public static SnapshotPoint? GetCaretPoint(this ITextView textView, Predicate<ITextSnapshot> match)
    {
        var caret = textView.Caret.Position;
        var span = textView.BufferGraph.MapUpOrDownToFirstMatch(new SnapshotSpan(caret.BufferPosition, 0), match);
        if (span.HasValue)
        {
            return span.Value.Start;
        }
        else
        {
            return null;
        }
    }

    public static ITextBuffer? GetBufferContainingCaret(this ITextView textView, string contentType = StandardContentTypeNames.Text)
    {
        var point = textView.GetCaretPoint(s => s.ContentType.IsOfType(contentType));
        return point.HasValue ? point.Value.Snapshot.TextBuffer : null;
    }
}
