// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open CancellableTasks

[<Struct>]
type FSharpCodeFixContext(document: Document, span: TextSpan) =
    member _.Document = document
    member _.Span = span

module internal FSharpCodeFixContextHelpers =
    let getSourceTextAsync (context: FSharpCodeFixContext) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
            return! (context.Document.GetTextAsync cancellationToken)
        }

    let getSquigglyTextAsync context =
        cancellableTask {
            let! sourceText = getSourceTextAsync context
            return sourceText.GetSubText(context.Span).ToString()
        }

    let getErrorRangeAsync context =
        cancellableTask {
            let! sourceText = getSourceTextAsync context
            return RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)
        }
