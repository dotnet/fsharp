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

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "AddExplicitReturnType"); Shared>]
type internal AddExplicitReturnType [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static member isValidMethodWithoutTypeAnnotation (funcOrValue: FSharpMemberOrFunctionOrValue) (symbolUse: FSharpSymbolUse) (parseFileResults:FSharpParseFileResults) =
        let isLambdaIfFunction =
            funcOrValue.IsFunction
            && parseFileResults.IsBindingALambdaAtPosition symbolUse.Range.Start

        (not funcOrValue.IsValue || not isLambdaIfFunction)
        && not (funcOrValue.ReturnParameter.Type.IsUnresolved)
        && not (parseFileResults.IsTypeAnnotationGivenAtPosition symbolUse.Range.Start)


    static member refactor (context:CodeRefactoringContext) (symbolUse:FSharpSymbolUse,memberFunc:FSharpMemberOrFunctionOrValue,symbolSpan:TextSpan,textLine:TextLine) =
        let typeString = memberFunc.FullType.FormatWithConstraints symbolUse.DisplayContext
        let title = SR.AddExplicitReturnTypeAnnotation()

        let getChangedText (sourceText: SourceText) =
            let debugInfo = $"{sourceText} : {typeString} : {symbolSpan} : {textLine}"
            debugInfo
            let sub = sourceText.ToString(textLine.Span)

            let newSub =
                sub.Replace("=", $" :{memberFunc.ReturnParameter.Type.TypeDefinition.DisplayName}=")

            sourceText.Replace(textLine.Span, newSub)

        let codeActionFunc = (fun (cancellationToken: CancellationToken) ->
            task  {
                let oldDocument = context.Document

                let! sourceText = context.Document.GetTextAsync(cancellationToken)
                let changedText = getChangedText sourceText

                let newDocument = context.Document.WithText(changedText)
                let! changes = newDocument.GetTextChangesAsync(oldDocument)
                Debugger.Log(0,"",$"{changes}")
                return newDocument
            }
        )
        let codeAction = 
                CodeAction.Create(
                    title,
                    codeActionFunc,
                    title
                )
                                

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


            let! (parseFileResults,checkFileResults) =
                document.GetFSharpParseAndCheckResultsAsync(nameof (AddExplicitReturnType))
                |> CancellableTask.start ct

            let res = 
                lexerSymbol
                |> Option.bind(fun lexer -> 
                    checkFileResults.GetSymbolUseAtLocation(
                    fcsTextLineNumber,
                    lexer.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    lexer.FullIsland
                ))
                |> Option.bind(fun symbolUse->
                        match symbolUse.Symbol with
                        | :? FSharpMemberOrFunctionOrValue as v 
                            when AddExplicitReturnType.isValidMethodWithoutTypeAnnotation v symbolUse parseFileResults->
                                Some (symbolUse,v)
                        | _ -> None
                )
                |> Option.bind(fun (symbolUse,memberFunc)->
                        match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.Range) with
                        | Some span -> Some(symbolUse,memberFunc,span)
                        | None -> None
                )
                |> Option.map(fun (symbolUse,memberFunc,textSpan) -> AddExplicitReturnType.refactor context (symbolUse,memberFunc,textSpan,textLine))

            return res
        }
