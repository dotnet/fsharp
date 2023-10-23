// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open CancellableTasks
open System.Diagnostics

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "AddExplicitReturnType"); Shared>]
type internal AddExplicitReturnType [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static member isValidMethodWithoutTypeAnnotation
        (funcOrValue: FSharpMemberOrFunctionOrValue)
        (symbolUse: FSharpSymbolUse)
        (parseFileResults: FSharpParseFileResults)
        =
        let typeAnnotationRange =
            parseFileResults.TryRangeOfReturnTypeHint(symbolUse.Range.Start, false)

        let isLambdaIfFunction =
            funcOrValue.IsFunction
            && parseFileResults.IsBindingALambdaAtPosition symbolUse.Range.Start

        (not funcOrValue.IsValue || not isLambdaIfFunction)
        && not (funcOrValue.ReturnParameter.Type.IsUnresolved)
        && not (parseFileResults.IsTypeAnnotationGivenAtPosition symbolUse.Range.Start)
        && not typeAnnotationRange.IsNone

    static member refactor
        (context: CodeRefactoringContext)
        (symbolUse: FSharpSymbolUse, memberFunc: FSharpMemberOrFunctionOrValue, parseFileResults: FSharpParseFileResults)
        =
        let title = SR.AddExplicitReturnTypeAnnotation()

        let getChangedText (sourceText: SourceText) =
            let inferredType = memberFunc.ReturnParameter.Type.TypeDefinition.DisplayName

            let rangeOfReturnType =
                parseFileResults.TryRangeOfReturnTypeHint(symbolUse.Range.Start, false)

            let textChange =
                rangeOfReturnType
                |> Option.map (fun range -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                |> Option.map (fun textSpan -> TextChange(textSpan, $":{inferredType}"))

            match textChange with
            | Some textChange -> sourceText.WithChanges(textChange)
            | None -> sourceText

        let codeActionFunc: CancellationToken -> Task<Document> =
            fun (cancellationToken: CancellationToken) ->
                task {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken)
                    let changedText = getChangedText sourceText

                    let newDocument = context.Document.WithText(changedText)
                    return newDocument
                }

        let codeAction = CodeAction.Create(title, codeActionFunc, title)

        do context.RegisterRefactoring(codeAction)

    override _.ComputeRefactoringsAsync context =
        backgroundTask {
            let document = context.Document
            let position = context.Span.Start
            let! sourceText = document.GetTextAsync()
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! ct = Async.CancellationToken

            let! lexerSymbol =
                document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof (AddExplicitReturnType))
                |> CancellableTask.start ct

            let! (parseFileResults, checkFileResults) =
                document.GetFSharpParseAndCheckResultsAsync(nameof (AddExplicitReturnType))
                |> CancellableTask.start ct

            let res =
                lexerSymbol
                |> Option.bind (fun lexer ->
                    checkFileResults.GetSymbolUseAtLocation(
                        fcsTextLineNumber,
                        lexer.Ident.idRange.EndColumn,
                        textLine.ToString(),
                        lexer.FullIsland
                    ))
                |> Option.bind (fun symbolUse ->
                    match symbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue as v when
                        AddExplicitReturnType.isValidMethodWithoutTypeAnnotation v symbolUse parseFileResults
                        ->
                        Some(symbolUse, v)
                    | _ -> None)
                |> Option.map (fun (symbolUse, memberFunc) ->
                    AddExplicitReturnType.refactor context (symbolUse, memberFunc, parseFileResults))

            return res
        }