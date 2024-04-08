// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveReturnOrYield); Shared>]
type internal RemoveReturnOrYieldCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0747", "FS0748")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! parseResults = document.GetFSharpParseResultsAsync(nameof (RemoveReturnOrYieldCodeFixProvider))

                let! sourceText = document.GetTextAsync(cancellationToken)

                let errorRange =
                    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)

                return
                    parseResults.TryRangeOfExprInYieldOrReturn errorRange.Start
                    |> Option.bind (fun exprRange -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, exprRange))
                    |> Option.map (fun exprSpan -> [ TextChange(span, sourceText.GetSubText(exprSpan).ToString()) ])
                    |> Option.map (fun changes ->
                        let title =
                            let text = sourceText.GetSubText(span).ToString()

                            if text.StartsWith("return!") then SR.RemoveReturnBang()
                            elif text.StartsWith("return") then SR.RemoveReturn()
                            elif text.StartsWith("yield!") then SR.RemoveYieldBang()
                            else SR.RemoveYield()

                        {
                            Name = CodeFix.RemoveReturnOrYield
                            Message = title
                            Changes = changes
                        })
            }
