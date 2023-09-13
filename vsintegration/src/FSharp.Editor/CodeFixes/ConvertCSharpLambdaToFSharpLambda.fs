// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertCSharpLambdaToFSharpLambda); Shared>]
type internal ConvertCSharpLambdaToFSharpLambdaCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.UseFSharpLambda()

    let tryGetSpans (parseResults: FSharpParseFileResults) (range: range) sourceText =
        parseResults.TryRangeOfParenEnclosingOpEqualsGreaterUsage range.Start
        |> ValueOption.ofOption
        |> ValueOption.map (fun (fullParenRange, lambdaArgRange, lambdaBodyRange) ->
            RoslynHelpers.FSharpRangeToTextSpan(sourceText, fullParenRange),
            RoslynHelpers.FSharpRangeToTextSpan(sourceText, lambdaArgRange),
            RoslynHelpers.FSharpRangeToTextSpan(sourceText, lambdaBodyRange))

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0039"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()

                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof ConvertCSharpLambdaToFSharpLambdaCodeFixProvider)
                let! sourceText = context.Document.GetTextAsync(cancellationToken)
                let! errorRange = context.GetErrorRangeAsync()

                return
                    tryGetSpans parseResults errorRange sourceText
                    |> ValueOption.map (fun (fullParenSpan, lambdaArgSpan, lambdaBodySpan) ->
                        let replacement =
                            let argText = sourceText.GetSubText(lambdaArgSpan).ToString()
                            let bodyText = sourceText.GetSubText(lambdaBodySpan).ToString()
                            TextChange(fullParenSpan, $"fun {argText} -> {bodyText}")

                        {
                            Name = CodeFix.ConvertCSharpLambdaToFSharpLambda
                            Message = title
                            Changes = [ replacement ]
                        })
            }
