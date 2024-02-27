namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "AddReturnType"); Shared>]
type internal AddReturnType [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static member isValidMethodWithoutTypeAnnotation
        (symbolUse: FSharpSymbolUse)
        (parseFileResults: FSharpParseFileResults)
        (funcOrValue: FSharpMemberOrFunctionOrValue)
        =
        let typeAnnotationRange =
            parseFileResults.TryRangeOfReturnTypeHint(symbolUse.Range.Start, false)

        let res =
            funcOrValue.CompiledName = funcOrValue.DisplayName
            && funcOrValue.IsFunction
            && not (parseFileResults.IsBindingALambdaAtPosition symbolUse.Range.Start)
            && not (funcOrValue.ReturnParameter.Type.IsUnresolved)
            && not (parseFileResults.IsTypeAnnotationGivenAtPosition symbolUse.Range.Start)
            && not typeAnnotationRange.IsNone

        match (res, typeAnnotationRange) with
        | (true, Some tr) -> Some(funcOrValue, tr)
        | (_, _) -> None

    static member refactor
        (context: CodeRefactoringContext)
        (memberFunc: FSharpMemberOrFunctionOrValue, typeRange: Range, symbolUse: FSharpSymbolUse)
        =
        let title = SR.AddReturnTypeAnnotation()

        let getChangedText (sourceText: SourceText) =
            let returnType = memberFunc.ReturnParameter.Type

            let inferredType =
                let res = returnType.Format symbolUse.DisplayContext

                if returnType.HasTypeDefinition then
                    res
                else
                    $"({res})".Replace(" ", "")

            let textSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, typeRange)
            let textChange = TextChange(textSpan, $": {inferredType} ")
            sourceText.WithChanges(textChange)

        let codeActionFunc =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = context.Document.GetTextAsync(cancellationToken)
                let changedText = getChangedText sourceText

                let newDocument = context.Document.WithText(changedText)
                return newDocument
            }

        let codeAction = CodeAction.Create(title, codeActionFunc, title)

        do context.RegisterRefactoring(codeAction)

    static member ofFSharpMemberOrFunctionOrValue(symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> Some v
        | _ -> None

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document
            let position = context.Span.Start
            let! sourceText = document.GetTextAsync()
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! lexerSymbol =
                document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof (AddReturnType))

            let! (parseFileResults, checkFileResults) = document.GetFSharpParseAndCheckResultsAsync(nameof (AddReturnType))

            let symbolUseOpt =
                lexerSymbol
                |> Option.bind (fun lexer ->
                    checkFileResults.GetSymbolUseAtLocation(
                        fcsTextLineNumber,
                        lexer.Ident.idRange.EndColumn,
                        textLine.ToString(),
                        lexer.FullIsland
                    ))

            let memberFuncOpt =
                symbolUseOpt
                |> Option.bind (fun sym -> sym.Symbol |> AddReturnType.ofFSharpMemberOrFunctionOrValue)

            match (symbolUseOpt, memberFuncOpt) with
            | (Some symbolUse, Some memberFunc) ->
                let isValidMethod =
                    memberFunc
                    |> AddReturnType.isValidMethodWithoutTypeAnnotation symbolUse parseFileResults

                match isValidMethod with
                | Some(memberFunc, typeRange) -> do AddReturnType.refactor context (memberFunc, typeRange, symbolUse)
                | None -> ()
            | _ -> ()

            return ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
