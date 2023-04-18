// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddParentheses); Shared>]
type internal FSharpWrapExpressionInParenthesesFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.WrapExpressionInParentheses()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0597")

    override this.RegisterCodeFixesAsync context : Task =
        backgroundTask {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let changes =
                [
                    TextChange(TextSpan(context.Span.Start, 0), "(")
                    TextChange(TextSpan(context.Span.End + 1, 0), ")")
                ]

            context.RegisterFsharpFix(CodeFix.AddParentheses, title, changes)
        }
