// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingRecToMutuallyRecFunctions); Shared>]
type internal FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let titleFormat = SR.MakeOuterBindingRecursive()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0576")

    interface IFSharpCodeFix with
        member _.GetChangesAsync document span =
            cancellableTask { 
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                
                let! defines, langVersion =
                    document.GetFSharpCompilationDefinesAndLangVersionAsync(
                        nameof (FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider)
                    )

                let! sourceText = document.GetTextAsync(cancellationToken)

                let funcStartPos =
                    let rec loop ch pos =
                        if not (Char.IsWhiteSpace(ch)) then
                            pos
                        else
                            loop sourceText.[pos + 1] (pos + 1)

                    loop sourceText.[span.End + 1] (span.End + 1)

                let funcLexerSymbol =
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

                let funcNameOpt = 
                    funcLexerSymbol
                    |> Option.bind (fun symbol -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbol.Range))
                    |> Option.map (fun funcNameSpan -> sourceText.GetSubText(funcNameSpan).ToString())

                return match funcNameOpt with
                       | Some funcName -> 
                            String.Format(titleFormat, funcName), [ TextChange(TextSpan(span.End, 0), " rec") ]
                       | None ->
                            "", []
            }

    override this.RegisterCodeFixesAsync context =
        //asyncMaybe {
        //    let! defines, langVersion =
        //        context.Document.GetFSharpCompilationDefinesAndLangVersionAsync(
        //            nameof (FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider)
        //        )
        //        |> liftAsync

        //    let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

        //    let funcStartPos =
        //        let rec loop ch pos =
        //            if not (Char.IsWhiteSpace(ch)) then
        //                pos
        //            else
        //                loop sourceText.[pos + 1] (pos + 1)

        //        loop sourceText.[context.Span.End + 1] (context.Span.End + 1)

        //    let! funcLexerSymbol =
        //        Tokenizer.getSymbolAtPosition (
        //            context.Document.Id,
        //            sourceText,
        //            funcStartPos,
        //            context.Document.FilePath,
        //            defines,
        //            SymbolLookupKind.Greedy,
        //            false,
        //            false,
        //            Some langVersion,
        //            context.CancellationToken
        //        )

        //    let! funcNameSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, funcLexerSymbol.Range)
        //    let funcName = sourceText.GetSubText(funcNameSpan).ToString()

        //    do
        //        context.RegisterFsharpFix(
        //            CodeFix.AddMissingRecToMutuallyRecFunctions,
        //            String.Format(titleFormat, funcName),
        //            [| TextChange(TextSpan(context.Span.End, 0), " rec") |]
        //        )
        //}
        cancellableTask {
            let! title, changes = (this :> IFSharpCodeFix).GetChangesAsync context.Document context.Span
            context.RegisterFsharpFix(CodeFix.AddMissingRecToMutuallyRecFunctions, title, changes)
        }
        |> CancellableTask.startAsTask context.CancellationToken

        //|> Async.Ignore
        //|> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
