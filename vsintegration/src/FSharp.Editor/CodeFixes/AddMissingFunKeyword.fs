// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingFunKeyword); Shared>]
type internal AddMissingFunKeywordCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.AddMissingFunKeyword()

    let adjustPosition (sourceText: SourceText) (span: TextSpan) =
        let rec loop ch pos =
            if not (Char.IsWhiteSpace(ch)) then
                pos
            else
                loop sourceText[pos] (pos - 1)

        loop (sourceText[span.Start - 1]) span.Start

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0010"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! textOfError = context.GetSquigglyTextAsync()

                if textOfError <> "->" then
                    return ValueNone
                else
                    let! cancellationToken = CancellableTask.getCancellationToken ()
                    let document = context.Document

                    let! defines, langVersion, strictIndentation =
                        document.GetFsharpParsingOptionsAsync(nameof AddMissingFunKeywordCodeFixProvider)

                    let! sourceText = context.GetSourceTextAsync()
                    let adjustedPosition = adjustPosition sourceText context.Span

                    return
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
                            strictIndentation,
                            cancellationToken
                        )
                        |> ValueOption.ofOption
                        |> ValueOption.map (fun intendedArgLexerSymbol ->
                            RoslynHelpers.FSharpRangeToTextSpan(sourceText, intendedArgLexerSymbol.Range))
                        |> ValueOption.map (fun intendedArgSpan ->
                            {
                                Name = CodeFix.AddMissingFunKeyword
                                Message = title
                                Changes = [ TextChange(TextSpan(intendedArgSpan.Start, 0), "fun ") ]
                            })
            }
