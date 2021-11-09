// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertToSingleEqualsEqualityExpression"); Shared>]
type internal FSharpConvertToSingleEqualsEqualityExpressionCodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set ["FS0043"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let text = sourceText.GetSubText(context.Span).ToString()

            // We're converting '==' into '=', a common new user mistake.
            // If this is an FS00043 that is anything other than that, bail out
            do! Option.guard (text = "==")

            let title = SR.ConvertToSingleEqualsEqualityExpression()

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(context.Span, "=") |]))

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)