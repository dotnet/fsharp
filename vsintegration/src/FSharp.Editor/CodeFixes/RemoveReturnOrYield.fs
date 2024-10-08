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
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof RemoveReturnOrYieldCodeFixProvider)

                let! sourceText = context.GetSourceTextAsync()
                let! errorRange = context.GetErrorRangeAsync()

                return
                    parseResults.TryRangeOfExprInYieldOrReturn errorRange.Start
                    |> ValueOption.ofOption
                    |> ValueOption.map (fun exprRange -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, exprRange))
                    |> ValueOption.map (fun exprSpan ->
                        [
                            // meaning: keyword + spacing before the expression
                            TextChange(TextSpan(context.Span.Start, exprSpan.Start - context.Span.Start), "")
                        ])
                    |> ValueOption.map (fun changes ->
                        let title =
                            let text = sourceText.GetSubText(context.Span).ToString()

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
