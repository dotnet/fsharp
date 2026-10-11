// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

open FSharp.Compiler.Syntax

open RefactoringHelpers
open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ExtractLetBinding"); Shared>]
type internal FSharpExtractLetBindingRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let register
        (context: CodeRefactoringContext)
        (sourceText: SourceText)
        (title: string)
        (kind: string)
        (changes: TextChange list)
        =
        let changedDocument =
            cancellableTask {
                TelemetryReporter.ReportSingleEvent(
                    TelemetryEvents.RefactoringActivated,
                    [| "name", box (nameof FSharpExtractLetBindingRefactoring); "kind", box kind |]
                )

                return context.Document.WithText(sourceText.WithChanges changes)
            }

        context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpExtractLetBindingRefactoring)

                let lambdaAtCaret =
                    if context.Span.IsEmpty then
                        tryParenthesizedLambdaAtCaret sourceText parseResults.ParseTree context.Span.Start
                    else
                        ValueNone

                let isSelected = not context.Span.IsEmpty || lambdaAtCaret.IsSome

                let target =
                    match lambdaAtCaret with
                    | ValueSome _ -> lambdaAtCaret
                    | ValueNone when context.Span.IsEmpty -> tryConstantAtCaret sourceText parseResults.ParseTree context.Span.Start
                    | ValueNone -> tryExtractionTarget sourceText parseResults.ParseTree context.Span

                match target with
                | ValueNone -> ()
                | ValueSome target ->
                    let! options = document.GetOptionsAsync cancellationToken

                    let indentSize =
                        options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                    let! defines, langVersion = document.GetFsharpParsingOptionsAsync(nameof FSharpExtractLetBindingRefactoring)
                    let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof FSharpExtractLetBindingRefactoring)
                    let names = usedNames parseResults.ParseTree

                    let literalLines =
                        linesInsideLiterals sourceText parseResults.ParseTree defines langVersion

                    let content =
                        match target.Expr with
                        | SynExpr.Paren(expr = inner) -> inner
                        | expr -> expr

                    if isSelected then
                        match anchorsOf target.Expr target.Path with
                        | anchor :: _ ->
                            let name = uniqueName "extracted" names

                            let functionType =
                                match content with
                                | SynExpr.Lambda _
                                | SynExpr.MatchLambda _ -> tryTypeText checkResults target.Expr.Range
                                | _ -> ValueNone

                            let header =
                                match functionType with
                                | ValueSome functionType -> $"let {name}: {functionType}"
                                | ValueNone -> $"let {name}"

                            match tryDeclareInFront sourceText target anchor header name indentSize literalLines with
                            | ValueSome changes -> register context sourceText (SR.ExtractToLetBinding()) "let" changes
                            | ValueNone -> ()
                        | [] -> ()

                    if isLiteralConstant content then
                        let name = uniqueName "ExtractedConstant" names
                        let attribute = literalAttributeAt checkResults target.Expr.Range.Start

                        match
                            tryDeclareInFrontOfModuleLet sourceText target [ $"[<{attribute}>]" ] $"let {name}" name indentSize literalLines
                        with
                        | ValueSome changes -> register context sourceText (SR.ExtractToLiteral()) "literal" changes
                        | ValueNone -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
