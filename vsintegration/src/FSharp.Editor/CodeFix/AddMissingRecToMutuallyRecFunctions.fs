// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Composition
open System.Threading

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingRecToMutuallyRecFunctions); Shared>]
type internal FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let titleFormat = SR.MakeOuterBindingRecursive()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0576")

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let! defines =
                context.Document.GetFSharpCompilationDefinesAsync(nameof (FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider))
                |> liftAsync

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let funcStartPos =
                let rec loop ch pos =
                    if not (Char.IsWhiteSpace(ch)) then
                        pos
                    else
                        loop sourceText.[pos + 1] (pos + 1)

                loop sourceText.[context.Span.End + 1] (context.Span.End + 1)

            let! funcLexerSymbol =
                Tokenizer.getSymbolAtPosition (
                    context.Document.Id,
                    sourceText,
                    funcStartPos,
                    context.Document.FilePath,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    context.CancellationToken
                )

            let! funcNameSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, funcLexerSymbol.Range)
            let funcName = sourceText.GetSubText(funcNameSpan).ToString()

            do
                context.RegisterFsharpFix(
                    CodeFix.AddMissingRecToMutuallyRecFunctions,
                    String.Format(titleFormat, funcName),
                    [| TextChange(TextSpan(context.Span.End, 0), " rec") |]
                )
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
