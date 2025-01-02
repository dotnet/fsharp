// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddNewKeyword); Shared>]
type internal AddNewKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddNewKeyword()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0760"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()
                let! parseFileResults = context.Document.GetFSharpParseResultsAsync(nameof AddNewKeywordCodeFixProvider)

                let getSourceLineStr line =
                    sourceText.Lines[Line.toZ line].ToString()

                let range =
                    RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)

                // Constructor arg
                // Qualified.Constructor arg
                // Constructor<TypeArg> arg
                // Qualified.Constructor<TypeArg> arg
                let matchingApp path node =
                    let (|TargetTy|_|) expr =
                        match expr with
                        | SynExpr.Ident id -> Some(SynType.LongIdent(SynLongIdent([ id ], [], [])))
                        | SynExpr.LongIdent(longDotId = longDotId) -> Some(SynType.LongIdent longDotId)
                        | SynExpr.TypeApp(SynExpr.Ident id, lessRange, typeArgs, commaRanges, greaterRange, _, range) ->
                            Some(
                                SynType.App(
                                    SynType.LongIdent(SynLongIdent([ id ], [], [])),
                                    Some lessRange,
                                    typeArgs,
                                    commaRanges,
                                    greaterRange,
                                    false,
                                    range
                                )
                            )
                        | SynExpr.TypeApp(SynExpr.LongIdent(longDotId = longDotId), lessRange, typeArgs, commaRanges, greaterRange, _, range) ->
                            Some(
                                SynType.App(SynType.LongIdent longDotId, Some lessRange, typeArgs, commaRanges, greaterRange, false, range)
                            )
                        | _ -> None

                    match node with
                    | SyntaxNode.SynExpr(SynExpr.App(funcExpr = TargetTy targetTy; argExpr = argExpr; range = m)) when
                        m |> Range.equals range
                        ->
                        Some(targetTy, argExpr, path)
                    | _ -> None

                match (range.Start, parseFileResults.ParseTree) ||> ParsedInput.tryPick matchingApp with
                | None -> return ValueNone
                | Some(targetTy, argExpr, path) ->
                    // Adding `new` may require additional parentheses: https://github.com/dotnet/fsharp/issues/15622
                    let needsParens =
                        let newExpr = SynExpr.New(false, targetTy, argExpr, range)

                        argExpr
                        |> SynExpr.shouldBeParenthesizedInContext getSourceLineStr (SyntaxNode.SynExpr newExpr :: path)

                    let newText =
                        let targetTyText =
                            sourceText.ToString(RoslynHelpers.FSharpRangeToTextSpan(sourceText, targetTy.Range))

                        // Constructor namedArg  → new Constructor(namedArg)
                        // Constructor "literal" → new Constructor "literal"
                        // Constructor ()        → new Constructor ()
                        // Constructor()         → new Constructor()
                        // Constructor           → new Constructor
                        // ····indentedArg         ····(indentedArg)
                        let textBetween =
                            let range =
                                Range.mkRange context.Document.FilePath targetTy.Range.End argExpr.Range.Start

                            if needsParens && range.StartLine = range.EndLine then
                                ""
                            else
                                sourceText.ToString(RoslynHelpers.FSharpRangeToTextSpan(sourceText, range))

                        let argExprText =
                            let originalArgText =
                                sourceText.ToString(RoslynHelpers.FSharpRangeToTextSpan(sourceText, argExpr.Range))

                            if needsParens then
                                $"(%s{originalArgText})"
                            else
                                originalArgText

                        $"new %s{targetTyText}%s{textBetween}%s{argExprText}"

                    return
                        ValueSome
                            {
                                Name = CodeFix.AddNewKeyword
                                Message = title
                                Changes = [ TextChange(context.Span, newText) ]
                            }
            }
