// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable
open System.Composition
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open CancellableTasks

[<Sealed>]
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeToUpcast); Shared>]
type internal AddMissingSeqCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingSeq()
    static let fixableDiagnosticIds = ImmutableArray.Create("FS3873", "FS0740")

    override _.FixableDiagnosticIds = fixableDiagnosticIds
    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this
    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()
                let! parseFileResults = context.Document.GetFSharpParseResultsAsync(nameof AddMissingSeqCodeFixProvider)

                let getSourceLineStr line =
                    sourceText.Lines[Line.toZ line].ToString()

                let range =
                    RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)

                let needsParens =
                    (range.Start, parseFileResults.ParseTree)
                    ||> ParsedInput.exists (fun path node ->
                        match path, node with
                        | SyntaxNode.SynExpr outer :: _, SyntaxNode.SynExpr(expr & SynExpr.ComputationExpr _) when
                            expr.Range |> Range.equals range
                            ->
                            let seqRange =
                                range
                                |> Range.withEnd (Position.mkPos range.Start.Line (range.Start.Column + 3))

                            let inner =
                                SynExpr.App(
                                    ExprAtomicFlag.NonAtomic,
                                    false,
                                    SynExpr.Ident(Ident(nameof seq, seqRange)),
                                    expr,
                                    Range.unionRanges seqRange expr.Range
                                )

                            let outer =
                                match outer with
                                | SynExpr.App(flag, isInfix, funcExpr, _, outerAppRange) ->
                                    SynExpr.App(flag, isInfix, funcExpr, inner, outerAppRange)
                                | outer -> outer

                            inner
                            |> SynExpr.shouldBeParenthesizedInContext getSourceLineStr (SyntaxNode.SynExpr outer :: path)
                        | _ -> false)

                let text = sourceText.ToString(TextSpan(context.Span.Start, context.Span.Length))
                let newText = if needsParens then $"(seq {text})" else $"seq {text}"

                return
                    ValueSome
                        {
                            Name = CodeFix.AddMissingSeq
                            Message = title
                            Changes = [ TextChange(context.Span, newText) ]
                        }
            }
