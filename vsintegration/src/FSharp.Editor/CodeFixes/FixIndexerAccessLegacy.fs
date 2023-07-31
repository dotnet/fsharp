// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open FSharp.Compiler.Diagnostics

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.FixIndexerAccess); Shared>]
type internal LegacyFixAddDotToIndexerAccessCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.AddIndexerDot

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS3217")

    override _.RegisterCodeFixesAsync context : Task =
        async {
            let! sourceText = context.Document.GetTextAsync() |> Async.AwaitTask

            context.Diagnostics
            |> Seq.iter (fun diagnostic ->

                let span, replacement =
                    try
                        let mutable span = context.Span

                        let notStartOfBracket (span: TextSpan) =
                            let t = sourceText.GetSubText(TextSpan(span.Start, span.Length + 1))
                            t.[t.Length - 1] <> '['

                        // skip all braces and blanks until we find [
                        while span.End < sourceText.Length && notStartOfBracket span do
                            span <- TextSpan(span.Start, span.Length + 1)

                        span, sourceText.GetSubText(span).ToString()
                    with _ ->
                        context.Span, sourceText.GetSubText(context.Span).ToString()

                do
                    context.RegisterFsharpFix(
                        CodeFix.FixIndexerAccess,
                        title,
                        [| TextChange(span, replacement.TrimEnd() + ".") |],
                        ImmutableArray.Create(diagnostic)
                    ))
        }
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
