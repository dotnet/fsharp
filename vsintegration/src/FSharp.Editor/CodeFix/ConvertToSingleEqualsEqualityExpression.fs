// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertToSingleEqualsEqualityExpression); Shared>]
type internal FSharpConvertToSingleEqualsEqualityExpressionCodeFixProvider() =
    inherit CodeFixProvider()
    
    static let title = SR.ConvertToSingleEqualsEqualityExpression()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0043")

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let text = sourceText.GetSubText(context.Span).ToString()

            // We're converting '==' into '=', a common new user mistake.
            // If this is an FS00043 that is anything other than that, bail out
            do! Option.guard (text = "==")
            do context.RegisterFsharpFix(CodeFix.ConvertToSingleEqualsEqualityExpression, title, [| TextChange(context.Span, "=") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
