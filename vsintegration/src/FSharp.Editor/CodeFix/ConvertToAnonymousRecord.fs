// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertToAnonymousRecord); Shared>]
type internal FSharpConvertToAnonymousRecordCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.ConvertToAnonymousRecord()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIsAppliesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpConvertToAnonymousRecordCodeFixProvider))

                let! sourceText = document.GetTextAsync(cancellationToken)

                let errorRange =
                    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)

                return
                    parseResults.TryRangeOfRecordExpressionContainingPos errorRange.Start
                    |> Option.bind (fun range -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range))
                    |> Option.map (fun span ->
                        {
                            Name = CodeFix.ConvertToAnonymousRecord
                            Message = title
                            Changes =
                                [
                                    TextChange(TextSpan(span.Start + 1, 0), "|")
                                    TextChange(TextSpan(span.End - 1, 0), "|")
                                ]
                        })
            }
