// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertToNotEqualsEqualityExpression); Shared>]
type internal FSharpConvertToNotEqualsEqualityExpressionCodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set [ "FS0043" ]
    static let title = SR.ConvertToNotEqualsEqualityExpression()

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let text = sourceText.GetSubText(context.Span).ToString()

            // We're converting '!=' into '<>', a common new user mistake.
            // If this is an FS00043 that is anything other than that, bail out
            do! Option.guard (text = "!=")
            do context.RegisterFsharpFix(CodeFix.ConvertToNotEqualsEqualityExpression, title, [| TextChange(context.Span, "<>") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
