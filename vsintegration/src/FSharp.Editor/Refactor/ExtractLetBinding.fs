// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.CodeRefactorings

open FSharp.Compiler.Syntax

open RefactoringHelpers
open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ExtractLetBinding"); Shared>]
type internal FSharpExtractLetBindingRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let letBindingChanges (extraction: Extraction) =
        match anchorsOf extraction.Target.Expr extraction.Target.Path with
        | [] -> ValueNone
        | anchor :: _ ->
            let name = uniqueName "extracted" extraction.Names

            let header =
                match tryFunctionTypeText extraction.CheckResults extraction.Target with
                | ValueSome functionType -> $"let {name}: {functionType}"
                | ValueNone -> $"let {name}"

            tryDeclareInFront extraction anchor header name

    static let literalChanges (extraction: Extraction) =
        let constant =
            match extraction.Target.Expr with
            | SynExpr.Paren(expr = inner) -> inner
            | expr -> expr

        if isLiteralConstant constant then
            let name = uniqueName "ExtractedConstant" extraction.Names

            let attribute =
                literalAttributeAt extraction.CheckResults extraction.Target.Expr.Range.Start

            tryDeclareInFrontOfModuleLet extraction [ $"[<{attribute}>]" ] $"let {name}" name
        else
            ValueNone

    static let offer (isSelected: bool) (extraction: Extraction) =
        if isSelected then
            registerRefactoring extraction (SR.ExtractToLetBinding()) "let" (letBindingChanges extraction)

        registerRefactoring extraction (SR.ExtractToLiteral()) "literal" (literalChanges extraction)

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not document.IsFSharpSignatureFile then
                let! sourceText = document.GetTextAsync context.CancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpExtractLetBindingRefactoring)
                let parseTree = parseResults.ParseTree
                let selected = trySelectedTarget sourceText parseTree context.Span

                let target =
                    selected
                    |> ValueOption.orElseWith (fun () -> tryConstantAtCaret sourceText parseTree context.Span)

                match target with
                | ValueNone -> ()
                | ValueSome target ->
                    let! extraction = prepareExtraction context (nameof FSharpExtractLetBindingRefactoring) sourceText parseTree target
                    offer selected.IsSome extraction
        }
        |> CancellableTask.startAsTask context.CancellationToken
