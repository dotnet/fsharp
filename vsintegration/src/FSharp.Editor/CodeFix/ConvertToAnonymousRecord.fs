// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertToAnonymousRecord); Shared>]
type internal FSharpConvertToAnonymousRecordCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039")

    interface IFSharpCodeFix with
        member _.GetChangesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpConvertToAnonymousRecordCodeFixProvider))

                let! sourceText = document.GetTextAsync(cancellationToken)

                let errorRange =
                    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)

                let changes =
                    parseResults.TryRangeOfRecordExpressionContainingPos errorRange.Start
                    |> Option.bind (fun range -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range))
                    |> Option.map (fun span ->
                        [
                            TextChange(TextSpan(span.Start + 1, 0), "|")
                            TextChange(TextSpan(span.End - 1, 0), "|")
                        ])
                    |> Option.defaultValue []

                let title = SR.ConvertToAnonymousRecord()
                return title, changes
            }

    override this.RegisterCodeFixesAsync context : Task =
        cancellableTask {
            let! title, changes = (this :> IFSharpCodeFix).GetChangesAsync context.Document context.Span
            return context.RegisterFsharpFix(CodeFix.ConvertToAnonymousRecord, title, changes)
        }
        |> CancellableTask.startAsTask context.CancellationToken
