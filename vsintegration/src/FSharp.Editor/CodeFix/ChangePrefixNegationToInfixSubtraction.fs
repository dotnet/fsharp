﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangePrefixNegationToInfixSubtraction); Shared>]
type internal FSharpChangePrefixNegationToInfixSubtractionodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.ChangePrefixNegationToInfixSubtraction()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0003")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
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
            do context.RegisterFsharpFix(CodeFix.ChangePrefixNegationToInfixSubtraction, title, [| TextChange(TextSpan(pos, 1), "- ") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
