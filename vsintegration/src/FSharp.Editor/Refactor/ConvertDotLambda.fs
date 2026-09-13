// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition

open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
module private DotLambdaConversion =

    [<NoComparison; NoEquality>]
    type Conversion =
        {
            Title: string
            Direction: string
            Change: TextChange
        }

    let hasName (name: string) (ident: Ident) =
        String.Equals(ident.idText, name, StringComparison.Ordinal)

    // The shapes SyntaxTreeOps.pushUnaryArg accepts under `_.`, with the parameter as the head of the chain.
    let rec tryChainRoot expr =
        match expr with
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = root :: _ :: _; dotRanges = dot :: _)) when
            Position.posEq dot.Start root.idRange.End
            ->
            ValueSome root
        | SynExpr.DotGet(expr = inner)
        | SynExpr.DotIndexedGet(objectExpr = inner)
        | SynExpr.TypeApp(expr = inner)
        | SynExpr.App(flag = ExprAtomicFlag.Atomic; isInfix = false; funcExpr = inner) -> tryChainRoot inner
        | _ -> ValueNone

    let occurrencesOf (name: string) (body: SynExpr) =
        (0, [ SyntaxNode.SynExpr body ])
        ||> SyntaxNodes.fold (fun count _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.Ident ident)
            | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ident :: _)))
            | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _))) when hasName name ident -> count + 1
            | _ -> count)

    let parameterNameFor (body: SynExpr) =
        let used =
            (HashSet<string>(StringComparer.Ordinal), [ SyntaxNode.SynExpr body ])
            ||> SyntaxNodes.fold (fun names _ node ->
                match node with
                | SyntaxNode.SynExpr(SynExpr.Ident ident)
                | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = ident :: _)))
                | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident, _))) -> ignore (names.Add ident.idText)
                | _ -> ()

                names)

        Seq.initInfinite (fun i -> if i = 0 then "x" else $"x{i}")
        |> Seq.find (used.Contains >> not)

    let isInQuotation (path: SyntaxVisitorPath) =
        path
        |> List.exists (function
            | SyntaxNode.SynExpr(SynExpr.Quote _) -> true
            | _ -> false)

    let isAppliedDirectly (lambda: SynExpr) (path: SyntaxVisitorPath) =
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner) as paren) :: SyntaxNode.SynExpr(SynExpr.App(funcExpr = func)) :: _ ->
            obj.ReferenceEquals(inner, lambda) && obj.ReferenceEquals(func, paren)
        | SyntaxNode.SynExpr(SynExpr.App(funcExpr = func)) :: _ -> obj.ReferenceEquals(func, lambda)
        | _ -> false

    let spanOf (sourceText: SourceText) (m: range) =
        RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

    let isBlankBetween (sourceText: SourceText) start finish =
        start <= finish
        && String.IsNullOrWhiteSpace(sourceText.ToString(TextSpan.FromBounds(start, finish)))

    let toShorthand (sourceText: SourceText) (path: SyntaxVisitorPath) (lambda: SynExpr) (root: Ident) =
        let lambdaSpan = spanOf sourceText lambda.Range
        let rootEnd = (spanOf sourceText root.idRange).End

        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; leftParenRange = leftParen; rightParenRange = Some rightParen; range = parenRange) as paren) :: SyntaxNode.SynExpr(SynExpr.App(
            flag = ExprAtomicFlag.NonAtomic; isInfix = false; argExpr = arg)) :: _ when
            obj.ReferenceEquals(inner, lambda)
            && obj.ReferenceEquals(arg, paren)
            && isBlankBetween sourceText (spanOf sourceText leftParen).End lambdaSpan.Start
            && isBlankBetween sourceText lambdaSpan.End (spanOf sourceText rightParen).Start
            ->
            let chain = sourceText.ToString(TextSpan.FromBounds(rootEnd, lambdaSpan.End))
            TextChange(spanOf sourceText parenRange, $"_{chain}")
        | _ -> TextChange(TextSpan.FromBounds(lambdaSpan.Start, rootEnd), "_")

    let toLambda (sourceText: SourceText) (path: SyntaxVisitorPath) (body: SynExpr) (range: range) (trivia: SynExprDotLambdaTrivia) =
        let name = parameterNameFor body
        let lambda = $"fun {name} -> {name}"
        let underscoreStart = (spanOf sourceText trivia.UnderscoreRange).Start
        let dotStart = (spanOf sourceText trivia.DotRange).Start

        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren _) :: _
        | SyntaxNode.SynBinding _ :: _ -> TextChange(TextSpan.FromBounds(underscoreStart, dotStart), lambda)
        | _ ->
            let span = spanOf sourceText range
            let chain = sourceText.ToString(TextSpan.FromBounds(dotStart, span.End))
            TextChange(span, $"({lambda}{chain})")

    let tryFind (sourceText: SourceText) (position: pos) (parseTree: ParsedInput) =
        (position, parseTree)
        ||> ParsedInput.tryPickLast (fun path node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.Lambda(
                fromMethod = false
                parsedData = Some([ SynPat.Named(ident = SynIdent(parameter, _); isThisVal = false; accessibility = None) ], body)) as lambda) when
                not (isInQuotation path) && not (isAppliedDirectly lambda path)
                ->
                match tryChainRoot body with
                | ValueSome root when hasName parameter.idText root && occurrencesOf parameter.idText body = 1 ->
                    Some
                        {
                            Title = SR.ConvertToShorthandLambda()
                            Direction = "shorthand"
                            Change = toShorthand sourceText path lambda root
                        }
                | _ -> None

            | SyntaxNode.SynExpr(SynExpr.DotLambda(expr = body; range = range; trivia = trivia)) when
                not body.IsArbExprAndThusAlreadyReportedError
                ->
                Some
                    {
                        Title = SR.ConvertToFunLambda()
                        Direction = "lambda"
                        Change = toLambda sourceText path body range trivia
                    }

            | _ -> None)

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertDotLambda"); Shared>]
type internal FSharpConvertDotLambdaRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken

                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertDotLambdaRefactoring)

                let caret = sourceText.Lines.GetLinePosition context.Span.Start
                let position = Position.mkPos (Line.fromZ caret.Line) caret.Character

                match DotLambdaConversion.tryFind sourceText position parseResults.ParseTree with
                | Some conversion ->
                    let changedDocument =
                        cancellableTask {
                            TelemetryReporter.ReportSingleEvent(
                                TelemetryEvents.RefactoringActivated,
                                [|
                                    "name", box (nameof FSharpConvertDotLambdaRefactoring)
                                    "direction", box conversion.Direction
                                |]
                            )

                            return document.WithText(sourceText.WithChanges conversion.Change)
                        }

                    context.RegisterRefactoring(CodeAction.Create(conversion.Title, changedDocument, conversion.Title))
                | None -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
