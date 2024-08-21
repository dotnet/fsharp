// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ReplaceWithSuggestion); Shared>]
type internal ReplaceWithSuggestionCodeFixProvider [<ImportingConstructor>] (settings: EditorOptions) =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS1129", "FS0495")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do! Option.guard settings.CodeFixes.SuggestNamesForErrors

            let document = context.Document

            let! parseFileResults, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (ReplaceWithSuggestionCodeFixProvider))
                |> liftAsync

            // This is all needed to get a declaration list
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let unresolvedIdentifierText = sourceText.GetSubText(context.Span).ToString()
            let pos = context.Span.End
            let caretLinePos = sourceText.Lines.GetLinePosition(pos)
            let caretLine = sourceText.Lines.GetLineFromPosition(pos)
            let fcsCaretLineNumber = Line.fromZ caretLinePos.Line
            let lineText = caretLine.ToString()

            let partialName =
                QuickParse.GetPartialLongNameEx(lineText, caretLinePos.Character - 1)

            let declInfo =
                checkFileResults.GetDeclarationListInfo(Some parseFileResults, fcsCaretLineNumber, lineText, partialName)

            let addNames (addToBuffer: string -> unit) =
                for item in declInfo.Items do
                    addToBuffer item.NameInList

            for suggestion in CompilerDiagnostics.GetSuggestedNames addNames unresolvedIdentifierText do
                let replacement = PrettyNaming.NormalizeIdentifierBackticks suggestion

                do
                    context.RegisterFsharpFix(
                        CodeFix.ReplaceWithSuggestion,
                        CompilerDiagnostics.GetErrorMessage(FSharpDiagnosticKind.ReplaceWithSuggestion suggestion),
                        [| TextChange(context.Span, replacement) |]
                    )
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
