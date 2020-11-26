// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ReplaceWithSuggestion"); Shared>]
type internal FSharpReplaceWithSuggestionCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager,
        settings: EditorOptions
    ) =
    inherit CodeFixProvider()

    static let userOpName = "ReplaceWithSuggestionCodeFix"
    let fixableDiagnosticIds = set ["FS0039"; "FS1129"; "FS0495"]
    let checker = checkerProvider.Checker
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do! Option.guard settings.CodeFixes.SuggestNamesForErrors

            let document = context.Document
            let! _, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken, userOpName)
            let! parseFileResults, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, userOpName=userOpName)

            // This is all needed to get a declaration list
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let unresolvedIdentifierText = sourceText.GetSubText(context.Span).ToString()
            let pos = context.Span.End
            let caretLinePos = sourceText.Lines.GetLinePosition(pos)            
            let caretLine = sourceText.Lines.GetLineFromPosition(pos)
            let fcsCaretLineNumber = Line.fromZ caretLinePos.Line
            let partialName = QuickParse.GetPartialLongNameEx(caretLine.ToString(), caretLinePos.Character - 1)
                
            let declInfo = checkFileResults.GetDeclarationListInfo(Some parseFileResults, fcsCaretLineNumber, caretLine.ToString(), partialName)
            let addNames (addToBuffer:string -> unit) = 
                for item in declInfo.Items do
                    addToBuffer item.Name

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            for suggestion in ErrorResolutionHints.getSuggestedNames addNames unresolvedIdentifierText do
                let replacement = Keywords.QuoteIdentifierIfNeeded suggestion
                let codeFix =
                    CodeFixHelpers.createTextChangeCodeFix(
                        CompilerDiagnostics.getErrorMessage (ReplaceWithSuggestion suggestion),
                        context,
                        (fun () -> asyncMaybe.Return [| TextChange(context.Span, replacement) |]))
                
                context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
