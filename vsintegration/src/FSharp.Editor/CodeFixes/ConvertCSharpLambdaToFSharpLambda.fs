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
        let flatten3 options =
            match options with
            | Some (Some a, Some b, Some c) -> Some(a, b, c)
            | _ -> None

        parseResults.TryRangeOfParenEnclosingOpEqualsGreaterUsage range.Start
        |> Option.map (fun (fullParenRange, lambdaArgRange, lambdaBodyRange) ->
            RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, fullParenRange),
            RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, lambdaArgRange),
            RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, lambdaBodyRange))
        |> flatten3

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0039"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof ConvertCSharpLambdaToFSharpLambdaCodeFixProvider)
                let! sourceText = context.Document.GetTextAsync(cancellationToken)
                let! errorRange = context.GetErrorRangeAsync()

                return
                    tryGetSpans parseResults errorRange sourceText
                    |> Option.map (fun (fullParenSpan, lambdaArgSpan, lambdaBodySpan) ->
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
