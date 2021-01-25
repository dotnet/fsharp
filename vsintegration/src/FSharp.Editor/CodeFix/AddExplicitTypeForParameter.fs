// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Collections.Immutable

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open FSharp.Compiler.Text
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
module internal CodeActionHelpers =
    let createTextChangeCodeFix (title: string, context: CodeRefactoringContext, computeTextChanges: unit -> Async<TextChange[] option>) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! changesOpt = computeTextChanges()
                    match changesOpt with
                    | None -> return context.Document
                    | Some textChanges -> return context.Document.WithText(sourceText.WithChanges(textChanges))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "AddExplicitTypeToParameter"); Shared>]
type internal FSharpAddExplicitTypeToParameterRefactoring
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeRefactoringProvider()

    static let userOpName = "AddExplicitTypeToParameter"

    override _.ComputeRefactoringsAsync context =
        asyncMaybe {
            let document = context.Document
            let position = context.Span.Start
            let checker = checkerProvider.Checker
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, CancellationToken.None, userOpName)
            let! sourceText = document.GetTextAsync () |> liftTaskAsync
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let _textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let _fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! parseFileResults, _, checkFileResults = checker.ParseAndCheckDocument (document, projectOptions, sourceText=sourceText, userOpName=userOpName)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
            // TODO - this is what I'd like to use, but can't ... I think
            //let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)
            let symbolUses =
                checkFileResults.GetAllUsesOfAllSymbolsInFile(context.CancellationToken)
                |> Seq.filter (fun su -> Range.rangeContainsPos su.RangeAlternate lexerSymbol.Range.End)
                |> Seq.toArray


            let isValidValueWithoutExplicitType (funcOrValue: FSharpMemberOrFunctionOrValue) (symbolUse: FSharpSymbolUse) =
                let isLambdaIfFunction =
                    funcOrValue.IsFunction &&
                    parseFileResults.IsBindingALambdaAtPosition symbolUse.RangeAlternate.Start

                (funcOrValue.IsValue || isLambdaIfFunction) &&
                parseFileResults.IsPositionContainedInACurriedParameter symbolUse.RangeAlternate.Start &&
                not (parseFileResults.IsTypeAnnotationGivenAtPosition symbolUse.RangeAlternate.Start) &&
                symbolUse.IsFromDefinition &&
                not funcOrValue.IsMember &&
                not funcOrValue.IsMemberThisValue &&
                not funcOrValue.IsConstructorThisValue &&
                not (PrettyNaming.IsOperatorName funcOrValue.DisplayName)

            for symbolUse in symbolUses do
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as v when isValidValueWithoutExplicitType v symbolUse ->

                    // TODO - need one that retains constraints
                    let turd = v.FullType.FormatWithConstraints symbolUse.DisplayContext
                    let title = "Add type annotation"

                    let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
                    
                    
                    // TODO: Handle if there were already parens
                    let getChangedText (sourceText: SourceText) =

                        sourceText.WithChanges(TextChange(TextSpan(symbolSpan.Start, 0), "("))
                                    .WithChanges(TextChange(TextSpan(symbolSpan.End + 1, 0), ": " + turd + ")"))

                    let codeAction =
                        CodeAction.Create(
                            title,
                            (fun (cancellationToken: CancellationToken) ->
                                async {
                                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                                    return context.Document.WithText(getChangedText sourceText)
                                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
                            title)
                    context.RegisterRefactoring(codeAction)
                | _ -> ()
        }
        |> Async.Ignore
        |>RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
