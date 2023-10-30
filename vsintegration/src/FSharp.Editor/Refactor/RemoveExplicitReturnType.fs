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

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open CancellableTasks
open System.Diagnostics
open Microsoft.CodeAnalysis.Text
open InternalOptionBuilder

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "RemoveExplicitReturnType"); Shared>]
type internal RemoveExplicitReturnType [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static member RangeOfReturnTypeDefinition(input: ParsedInput, symbolUseStart: pos, ?skipLambdas) =
        let skipLambdas = defaultArg skipLambdas true

        SyntaxTraversal.Traverse(
            symbolUseStart,
            input,
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) = defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding(expr = SynExpr.Lambda _) when skipLambdas -> defaultTraverse binding
                    | SynBinding(expr = SynExpr.DotLambda _) when skipLambdas -> defaultTraverse binding
                    ////I need the : before the Return Info
                    //| SynBinding(expr = SynExpr.Typed _)  -> defaultTraverse binding

                    // Dont skip manually type-annotated bindings
                    | SynBinding(returnInfo = Some (SynBindingReturnInfo (_, r, _, _))) -> Some r

                    // Let binding
                    | SynBinding (trivia = { EqualsRange = Some equalsRange }; range = range) when range.Start = symbolUseStart ->
                        Some equalsRange.StartRange

                    // Member binding
                    | SynBinding (headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = _ :: ident :: _))
                                  trivia = { EqualsRange = Some equalsRange }) when ident.idRange.Start = symbolUseStart ->
                        Some equalsRange.StartRange

                    | _ -> defaultTraverse binding
            }
        )

    static member RangeIncludingColon(range: TextSpan, sourceText: SourceText) =

        let lineUntilType = TextSpan.FromBounds(0, range.Start)

        let colonText = sourceText.GetSubText(lineUntilType).ToString()
        let newSpan = TextSpan.FromBounds(colonText.LastIndexOf(':'), range.End)
        newSpan

    static member isValidMethodWithoutTypeAnnotation
        (symbolUse: FSharpSymbolUse)
        (parseFileResults: FSharpParseFileResults)
        (funcOrValue: FSharpMemberOrFunctionOrValue)
        =
        let returnTypeHintAlreadyPresent =
            RemoveExplicitReturnType.RangeOfReturnTypeDefinition(parseFileResults.ParseTree, symbolUse.Range.Start, false)
            |> Option.isSome

        let isLambdaIfFunction =
            funcOrValue.IsFunction
            && parseFileResults.IsBindingALambdaAtPosition symbolUse.Range.Start

        let res =
            (not funcOrValue.IsValue || not isLambdaIfFunction)
            && returnTypeHintAlreadyPresent

        match res with
        | true -> Some funcOrValue
        | false -> None

    static member refactor
        (context: CodeRefactoringContext)
        (_: FSharpMemberOrFunctionOrValue)
        (parseFileResults: FSharpParseFileResults)
        (symbolUse: FSharpSymbolUse)
        =
        let title = SR.RemoveExplicitReturnTypeAnnotation()

        let getChangedText (sourceText: SourceText) =

            let newSourceText =
                RemoveExplicitReturnType.RangeOfReturnTypeDefinition(parseFileResults.ParseTree, symbolUse.Range.Start, false)
                |> Option.map (fun range -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                |> Option.map (fun textSpan -> RemoveExplicitReturnType.RangeIncludingColon(textSpan, sourceText))
                |> Option.map (fun textSpan -> sourceText.Replace(textSpan, ""))

            match newSourceText with
            | Some t -> t
            | None -> sourceText

        let codeActionFunc =
            (fun (cancellationToken: CancellationToken) ->
                task {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken)
                    let changedText = getChangedText sourceText

                    let newDocument = context.Document.WithText(changedText)
                    return newDocument
                })

        let codeAction = CodeAction.Create(title, codeActionFunc, title)

        do context.RegisterRefactoring(codeAction)

    static member ofFSharpMemberOrFunctionOrValue(symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> Some v
        | _ -> None

    override _.ComputeRefactoringsAsync context =
        let ct = context.CancellationToken

        cancellableTask {
            let document = context.Document
            let position = context.Span.Start
            let! sourceText = document.GetTextAsync()
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! lexerSymbol =
                document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof (RemoveExplicitReturnType))

            let! (parseFileResults, checkFileResults) = document.GetFSharpParseAndCheckResultsAsync(nameof (RemoveExplicitReturnType))

            let res =
                internalOption {
                    let! lexerSymbol = lexerSymbol

                    let! symbolUse =
                        checkFileResults.GetSymbolUseAtLocation(
                            fcsTextLineNumber,
                            lexerSymbol.Ident.idRange.EndColumn,
                            textLine.ToString(),
                            lexerSymbol.FullIsland
                        )

                    let! memberFunc =
                        symbolUse.Symbol |> RemoveExplicitReturnType.ofFSharpMemberOrFunctionOrValue
                        |>! RemoveExplicitReturnType.isValidMethodWithoutTypeAnnotation symbolUse parseFileResults

                    let res =
                        RemoveExplicitReturnType.refactor context memberFunc parseFileResults symbolUse

                    return res
                }

            return res
        }
        |> CancellableTask.startAsTask ct
