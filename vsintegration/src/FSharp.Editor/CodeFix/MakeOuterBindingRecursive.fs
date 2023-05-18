﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.MakeOuterBindingRecursive); Shared>]
type internal FSharpMakeOuterBindingRecursiveCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039")

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let! parseResults =
                context.Document.GetFSharpParseResultsAsync(nameof (FSharpMakeOuterBindingRecursiveCodeFixProvider))
                |> liftAsync

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let diagnosticRange =
                RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)

            do! Option.guard (parseResults.IsPosContainedInApplication diagnosticRange.Start)

            let! outerBindingRange = parseResults.TryRangeOfNameOfNearestOuterBindingContainingPos diagnosticRange.Start
            let! outerBindingNameSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, outerBindingRange)

            // One last check to verify the names are the same
            do!
                Option.guard (
                    sourceText
                        .GetSubText(outerBindingNameSpan)
                        .ContentEquals(sourceText.GetSubText(context.Span))
                )

            let title =
                String.Format(SR.MakeOuterBindingRecursive(), sourceText.GetSubText(outerBindingNameSpan).ToString())

            do
                context.RegisterFsharpFix(
                    CodeFix.MakeOuterBindingRecursive,
                    title,
                    [| TextChange(TextSpan(outerBindingNameSpan.Start, 0), "rec ") |]
                )
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
