// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints

open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree

[<Export(typeof<IFSharpInlineHintsService>)>]
type internal FSharpInlineHintsService [<ImportingConstructor>] (checkerProvider: FSharpCheckerProvider, projectInfoManager: FSharpProjectOptionsManager) =

    static let userOpName = "FSharpInlineHints"

    interface IFSharpInlineHintsService with
        member _.GetInlineHintsAsync(document: Document, _textSpan: TextSpan, cancellationToken: CancellationToken) =
            asyncMaybe {
                let! _parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync(cancellationToken)
                //let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
                //let _textLine = sourceText.Lines.GetLineFromPosition(textSpan.Start)
                //let _textLinePos = sourceText.Lines.GetLinePosition(textSpan.Start)
                //let _fcsTextLineNumber = Line.fromZ textLinePos.Line
                //let! _symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, textSpan.Start, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
                let! _parseFileResults, _, checkFileResults = checkerProvider.Checker.ParseAndCheckDocument(document, projectOptions, userOpName)
                let! symbols = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                //let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland, userOpName=userOpName)

                let hints =
                    [|
                        for symbol in symbols do
                            // let givenRange = RoslynHelpers.TextSpanToFSharpRange
                            let symbolSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, symbol.RangeAlternate)
                            FSharpInlineHint(TextSpan(symbolSpan.Start, 0), ImmutableArray.Create(TaggedText(TextTags.Text, symbol.Symbol.DisplayName + ":")))
                    |]

                return hints.ToImmutableArray()
            }
            |> Async.map (Option.defaultValue ImmutableArray<_>.Empty)
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)