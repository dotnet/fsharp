// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions

open FSharp.Compiler.SourceCodeServices
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
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! parseFileResults, _, checkFileResults = checker.ParseAndCheckDocument (document, projectOptions, sourceText=sourceText, userOpName=userOpName)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)

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

            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as v when isValidValueWithoutExplicitType v symbolUse ->
                let codeAction =
                    CodeActionHelpers.createTextChangeCodeFix(
                        "yeet",
                        context,
                        (fun () -> asyncMaybe.Return [| TextChange(TextSpan(lexerSymbol.Ident.idRange.EndColumn + 1, 1), ": int" + parameter.Type.Format symbolUse.DisplayContext) |]))
                context.RegisterRefactoring(codeAction)
            | _ -> ()
        }
        |> Async.Ignore
        |>RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
