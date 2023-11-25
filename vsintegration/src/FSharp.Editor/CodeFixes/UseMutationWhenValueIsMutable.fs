// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Symbols

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.UseMutationWhenValueIsMutable); Shared>]
type internal UseMutationWhenValueIsMutableCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.UseMutationWhenValueIsMutable()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0020"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let document = context.Document

                if isSignatureFile document.FilePath then
                    return ValueNone
                else
                    let! sourceText = context.GetSourceTextAsync()

                    let adjustedPosition =
                        let rec loop ch pos =
                            if
                                Char.IsWhiteSpace(ch)
                                // edge case - end of file
                                || pos = sourceText.Length - 1
                            then
                                pos
                            else
                                loop sourceText[pos + 1] (pos + 1)

                        loop sourceText[context.Span.Start] context.Span.Start

                    let! lexerSymbolOpt =
                        document.TryFindFSharpLexerSymbolAsync(
                            adjustedPosition,
                            SymbolLookupKind.Greedy,
                            false,
                            false,
                            nameof UseMutationWhenValueIsMutableCodeFixProvider
                        )

                    match lexerSymbolOpt with
                    | None -> return ValueNone
                    | Some lexerSymbol ->
                        let! fcsTextLineNumber, textLine = context.GetLineNumberAndText adjustedPosition

                        let! _, checkFileResults =
                            document.GetFSharpParseAndCheckResultsAsync(nameof UseMutationWhenValueIsMutableCodeFixProvider)

                        let symbolUseOpt =
                            checkFileResults.GetSymbolUseAtLocation(
                                fcsTextLineNumber,
                                lexerSymbol.Ident.idRange.EndColumn,
                                textLine,
                                lexerSymbol.FullIsland
                            )

                        let isValidCase (symbol: FSharpSymbol) =
                            match symbol with
                            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsMutable || mfv.HasSetterMethod -> true
                            | _ -> false

                        match symbolUseOpt with
                        | Some symbolUse when isValidCase symbolUse.Symbol ->
                            let symbolSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.Range)
                            let mutable pos = symbolSpan.End
                            let mutable ch = sourceText[pos]

                            // We're looking for the possibly erroneous '='
                            while pos <= context.Span.Length && ch <> '=' do
                                pos <- pos + 1
                                ch <- sourceText[pos]

                            return
                                ValueSome
                                    {
                                        Name = CodeFix.UseMutationWhenValueIsMutable
                                        Message = title
                                        Changes = [ TextChange(TextSpan(pos + 1, 1), "<-") ]
                                    }
                        | _ -> return ValueNone
            }
