// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.UseTripleQuotedInterpolation); Shared>]
type internal FSharpUseTripleQuotedInterpolationCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = [ "FS3373" ]
    static let title = SR.UseTripleQuotedInterpolation()
    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let! parseResults =
                context.Document.GetFSharpParseResultsAsync(nameof (FSharpUseTripleQuotedInterpolationCodeFixProvider))
                |> liftAsync

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let errorRange =
                RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)

            let! interpolationRange = parseResults.TryRangeOfStringInterpolationContainingPos errorRange.Start
            let! interpolationSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, interpolationRange)

            let replacement =
                let interpolation = sourceText.GetSubText(interpolationSpan).ToString()
                TextChange(interpolationSpan, "$\"\"" + interpolation.[1..] + "\"\"")

            do context.RegisterFsharpFix(CodeFix.UseTripleQuotedInterpolation, title, [| replacement |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
