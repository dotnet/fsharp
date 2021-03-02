// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ChangePrefixNegationToInfixSubtraction"); Shared>]
type internal FSharpChangePrefixNegationToInfixSubtractionodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set ["FS0003"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let mutable pos = context.Span.End + 1

            // This won't ever actually happen, but eh why not
            do! Option.guard (pos < sourceText.Length)

            let mutable ch = sourceText.[pos]
            while pos < sourceText.Length && Char.IsWhiteSpace(ch) do
                pos <- pos + 1
                ch <- sourceText.[pos]

            // Bail if this isn't a negation
            do! Option.guard (ch = '-')

            let title = SR.ChangePrefixNegationToInfixSubtraction()

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(TextSpan(pos, 1), "- ") |]))

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)  