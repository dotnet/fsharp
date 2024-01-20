// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.VisualStudio.FSharp.Editor.SymbolHelpers

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Tokenization.FSharpKeywords

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.FSharpRenameParamToMatchSignature); Shared>]
type internal RenameParamToMatchSignatureCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    let getSuggestion (d: Diagnostic) =
        let parts = Regex.Match(d.GetMessage(), ".+'(.+)'.+'(.+)'.+")

        if parts.Success then
            ValueSome parts.Groups.[1].Value
        else
            ValueNone

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3218"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()

                let suggestionOpt = getSuggestion context.Diagnostics[0]

                match suggestionOpt with
                | ValueSome suggestion ->
                    let replacement = NormalizeIdentifierBackticks suggestion

                    let! symbolUses = getSymbolUsesOfSymbolAtLocationInDocument (context.Document, context.Span.Start)
                    let symbolUses = symbolUses |> Option.defaultValue [||]

                    let changes =
                        [
                            for symbolUse in symbolUses do
                                let span = RoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.Range)
                                let textSpan = Tokenizer.fixupSpan (sourceText, span)
                                yield TextChange(textSpan, replacement)
                        ]

                    return
                        ValueSome
                            {
                                Name = CodeFix.FSharpRenameParamToMatchSignature
                                Message = CompilerDiagnostics.GetErrorMessage(FSharpDiagnosticKind.ReplaceWithSuggestion suggestion)
                                Changes = changes
                            }

                | ValueNone -> return ValueNone
            }
