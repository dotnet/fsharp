// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingRecToMutuallyRecFunctions); Shared>]
type internal AddMissingRecToMutuallyRecFunctionsCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let titleFormat = SR.MakeOuterBindingRecursive()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0576")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()

                let! defines, langVersion, strictIndentation =
                    context.Document.GetFsharpParsingOptionsAsync(nameof (AddMissingRecToMutuallyRecFunctionsCodeFixProvider))

                let! sourceText = context.GetSourceTextAsync()

                let funcStartPos =
                    let rec loop ch pos =
                        if not (Char.IsWhiteSpace(ch)) then
                            pos
                        else
                            loop sourceText.[pos + 1] (pos + 1)

                    loop sourceText.[context.Span.End + 1] (context.Span.End + 1)

                return
                    Tokenizer.getSymbolAtPosition (
                        context.Document.Id,
                        sourceText,
                        funcStartPos,
                        context.Document.FilePath,
                        defines,
                        SymbolLookupKind.Greedy,
                        false,
                        false,
                        Some langVersion,
                        strictIndentation,
                        cancellationToken
                    )
                    |> ValueOption.ofOption
                    |> ValueOption.map (fun funcLexerSymbol -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, funcLexerSymbol.Range))
                    |> ValueOption.map (fun funcNameSpan -> sourceText.GetSubText(funcNameSpan).ToString())
                    |> ValueOption.map (fun funcName ->
                        {
                            Name = CodeFix.AddMissingRecToMutuallyRecFunctions
                            Message = String.Format(titleFormat, funcName)
                            Changes = [ TextChange(TextSpan(context.Span.End, 0), " rec") ]
                        })
            }
