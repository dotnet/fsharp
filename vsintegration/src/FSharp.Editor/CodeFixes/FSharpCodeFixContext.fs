// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open CancellableTasks

type FSharpCodeFixContext(document: Document, span: TextSpan) =

    let sourceText =
        lazy
            (cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                return! (document.GetTextAsync cancellationToken)
            })

    member _.Document = document
    member _.Span = span

    member _.GetSourceTextAsync() = sourceText.Value

    member this.GetSquigglyTextAsync() =
        cancellableTask {
            let! sourceText = this.GetSourceTextAsync()
            return sourceText.GetSubText(span).ToString()
        }

    member this.GetErrorRangeAsync() =
        cancellableTask {
            let! sourceText = this.GetSourceTextAsync()
            return RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)
        }

    member this.GetParseResultsAsync userOpName =
        cancellableTask { return! this.Document.GetFSharpParseResultsAsync userOpName }
