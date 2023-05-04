// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingFunKeyword); Shared>]
type internal FSharpAddMissingFunKeywordCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()
    static let title = SR.AddMissingFunKeyword()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0010")

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let document = context.Document
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let textOfError = sourceText.GetSubText(context.Span).ToString()

            // Only trigger when failing to parse `->`, which arises when `fun` is missing
            do! Option.guard (textOfError = "->")

            let! defines, langVersion =
                document.GetFSharpCompilationDefinesAndLangVersionAsync(nameof (FSharpAddMissingFunKeywordCodeFixProvider))
                |> liftAsync

            let adjustedPosition =
                let rec loop ch pos =
                    if not (Char.IsWhiteSpace(ch)) then
                        pos
                    else
                        loop sourceText.[pos] (pos - 1)

                loop sourceText.[context.Span.Start - 1] context.Span.Start

            let! intendedArgLexerSymbol =
                Tokenizer.getSymbolAtPosition (
                    document.Id,
                    sourceText,
                    adjustedPosition,
                    document.FilePath,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    Some langVersion,
                    context.CancellationToken
                )

            let! intendedArgSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, intendedArgLexerSymbol.Range)

            do context.RegisterFsharpFix(CodeFix.AddMissingFunKeyword, title, [| TextChange(TextSpan(intendedArgSpan.Start, 0), "fun ") |])
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
