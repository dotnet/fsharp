namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "AddExplicitReturnType"); Shared>]
type internal AddExplicitReturnType [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static member isValidMethodWithoutTypeAnnotation
        (symbolUse: FSharpSymbolUse)
        (parseFileResults: FSharpParseFileResults)
        (funcOrValue: FSharpMemberOrFunctionOrValue)
        =
        let typeAnnotationRange =
            parseFileResults.TryRangeOfReturnTypeHint(symbolUse.Range.Start, false)

        let isLambdaIfFunction =
            funcOrValue.IsFunction
            && parseFileResults.IsBindingALambdaAtPosition symbolUse.Range.Start

        let res =
            funcOrValue.CompiledName = funcOrValue.DisplayName
            && not funcOrValue.IsValue
            && (not funcOrValue.IsValue || not isLambdaIfFunction)
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
                document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, nameof (AddExplicitReturnType))

            let! (parseFileResults, checkFileResults) = document.GetFSharpParseAndCheckResultsAsync(nameof (AddExplicitReturnType))

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
                |> Option.bind (fun sym -> sym.Symbol |> AddExplicitReturnType.ofFSharpMemberOrFunctionOrValue)

            match (symbolUseOpt, memberFuncOpt) with
            | (Some symbolUse, Some memberFunc) ->
                let isValidMethod =
                    memberFunc
                    |> AddExplicitReturnType.isValidMethodWithoutTypeAnnotation symbolUse parseFileResults

                match isValidMethod with
                | Some(memberFunc, typeRange) -> do AddExplicitReturnType.refactor context (memberFunc, typeRange, symbolUse)
                | None -> ()
            | _ -> ()

            return ()
        }
        |> CancellableTask.startAsTaskWithoutCancellation
