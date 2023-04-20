// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeRefCellDerefToNotExpression); Shared>]
type internal FSharpChangeRefCellDerefToNotExpressionCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.UseNotForNegation()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0001")

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document

            let! parseResults =
                document.GetFSharpParseResultsAsync(nameof (FSharpChangeRefCellDerefToNotExpressionCodeFixProvider))
                |> liftAsync

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let errorRange =
                RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)

            let! derefRange = parseResults.TryRangeOfRefCellDereferenceContainingPos errorRange.Start
            let! derefSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, derefRange)

            do context.RegisterFsharpFix(CodeFix.ChangeRefCellDerefToNotExpression, title, [| TextChange(derefSpan, "not ") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
