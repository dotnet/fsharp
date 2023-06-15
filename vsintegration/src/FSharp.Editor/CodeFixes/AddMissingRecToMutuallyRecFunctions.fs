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
        member _.GetCodeFixIfAppliesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! defines, langVersion =
                    document.GetFSharpCompilationDefinesAndLangVersionAsync(
                        nameof (AddMissingRecToMutuallyRecFunctionsCodeFixProvider)
                    )

                let! sourceText = document.GetTextAsync(cancellationToken)

                let funcStartPos =
                    let rec loop ch pos =
                        if not (Char.IsWhiteSpace(ch)) then
                            pos
                        else
                            loop sourceText.[pos + 1] (pos + 1)

                    loop sourceText.[span.End + 1] (span.End + 1)

                return
                    Tokenizer.getSymbolAtPosition (
                        document.Id,
                        sourceText,
                        funcStartPos,
                        document.FilePath,
                        defines,
                        SymbolLookupKind.Greedy,
                        false,
                        false,
                        Some langVersion,
                        cancellationToken
                    )
                    |> Option.bind (fun funcLexerSymbol -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, funcLexerSymbol.Range))
                    |> Option.map (fun funcNameSpan -> sourceText.GetSubText(funcNameSpan).ToString())
                    |> Option.map (fun funcName ->
                        {
                            Name = CodeFix.AddMissingRecToMutuallyRecFunctions
                            Message = String.Format(titleFormat, funcName)
                            Changes = [ TextChange(TextSpan(span.End, 0), " rec") ]
                        })
            }
