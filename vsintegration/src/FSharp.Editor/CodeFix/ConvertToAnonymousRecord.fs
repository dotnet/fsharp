// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertToAnonymousRecord); Shared>]
type internal FSharpConvertToAnonymousRecordCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.ConvertToAnonymousRecord()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document

            let! parseResults =
                document.GetFSharpParseResultsAsync(nameof (FSharpConvertToAnonymousRecordCodeFixProvider))
                |> liftAsync

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let errorRange =
                RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)

            let! recordRange = parseResults.TryRangeOfRecordExpressionContainingPos errorRange.Start
            let! recordSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, recordRange)

            let changes =
                [
                    TextChange(TextSpan(recordSpan.Start + 1, 0), "|")
                    TextChange(TextSpan(recordSpan.End, 0), "|")
                ]

            context.RegisterFsharpFix(CodeFix.ConvertToAnonymousRecord, title, changes)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
