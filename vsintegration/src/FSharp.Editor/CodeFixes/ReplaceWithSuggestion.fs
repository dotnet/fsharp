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

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ReplaceWithSuggestion); Shared>]
type internal ReplaceWithSuggestionCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0495")

    override this.RegisterCodeFixesAsync context =
        if context.Document.Project.IsFSharpCodeFixesSuggestNamesForErrorsEnabled then
            context.RegisterFsharpFixes this
        else
            Task.CompletedTask

    interface IFSharpMultiCodeFixProvider with
        member _.GetCodeFixesAsync context =
            cancellableTask {
                let! parseFileResults, checkFileResults =
                    context.Document.GetFSharpParseAndCheckResultsAsync(nameof ReplaceWithSuggestionCodeFixProvider)

                let! sourceText = context.GetSourceTextAsync()
                let! unresolvedIdentifierText = context.GetSquigglyTextAsync()
                let pos = context.Span.End
                let caretLinePos = sourceText.Lines.GetLinePosition(pos)
                let! fcsCaretLineNumber, lineText = context.GetLineNumberAndText pos

                let partialName =
                    QuickParse.GetPartialLongNameEx(lineText, caretLinePos.Character - 1)

                let declInfo =
                    checkFileResults.GetDeclarationListInfo(Some parseFileResults, fcsCaretLineNumber, lineText, partialName)

                let addNames addToBuffer =
                    for item in declInfo.Items do
                        addToBuffer item.NameInList

                return
                    CompilerDiagnostics.GetSuggestedNames addNames unresolvedIdentifierText
                    |> Seq.map (fun suggestion ->
                        let replacement = PrettyNaming.NormalizeIdentifierBackticks suggestion

                        {
                            Name = CodeFix.ReplaceWithSuggestion
                            Message = CompilerDiagnostics.GetErrorMessage(FSharpDiagnosticKind.ReplaceWithSuggestion suggestion)
                            Changes = [ TextChange(context.Span, replacement) ]
                        })
            }
