// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open FSharp.Compiler.Diagnostics

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.FixIndexerAccess); Shared>]
type internal LegacyFsharpFixAddDotToIndexerAccess() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set [ "FS3217" ]
    static let title = CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.AddIndexerDot

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

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

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveIndexerDotBeforeBracket); Shared>]
type internal FsharpFixRemoveDotFromIndexerAccessOptIn() as this =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set [ "FS3366" ]

    static let title =
        CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.RemoveIndexerDot

    member this.GetChanges(_document: Document, diagnostics: ImmutableArray<Diagnostic>, _ct: CancellationToken) =
        backgroundTask {
            let changes =
                diagnostics |> Seq.map (fun x -> TextChange(x.Location.SourceSpan, ""))

            return changes
        }

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
            ctx.RegisterFsharpFix(CodeFix.RemoveIndexerDotBeforeBracket, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.RemoveIndexerDotBeforeBracket this.GetChanges
