// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks
open TupleConversion

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertTuple"); Shared>]
type internal FSharpConvertTupleRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let hasSignatureFile (document: Document) =
        let signaturePath = document.FilePath + "i"

        document.Project.Documents
        |> Seq.exists (fun d -> String.Equals(d.FilePath, signaturePath, StringComparison.OrdinalIgnoreCase))

    static let isInQuotation (caretNode: CaretNode) =
        let path =
            match caretNode with
            | CaretNode.Expr(path = path)
            | CaretNode.Pat(path = path)
            | CaretNode.Type(annotated = Annotated.Pattern(path = path))
            | CaretNode.Type(annotated = Annotated.Expression(path = path)) -> path
            | CaretNode.Type _ -> []

        path
        |> List.exists (function
            | SyntaxNode.SynExpr(SynExpr.Quote _) -> true
            | _ -> false)

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not (document.IsFSharpSignatureFile || hasSignatureFile document) then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertTupleRefactoring)

                let caret =
                    let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                    Position.mkPos (Line.fromZ linePosition.Line) linePosition.Character

                match tryCaretNode caret parseResults.ParseTree with
                | ValueSome caretNode when not (isInQuotation caretNode) ->
                    let title =
                        if isStructNode caretNode then
                            SR.ConvertToReferenceTuple()
                        else
                            SR.ConvertToStructTuple()

                    let changedSolution =
                        cancellableTask {
                            let! converted = TuplePropagation.tryConvert document caretNode (nameof FSharpConvertTupleRefactoring)

                            return
                                match converted with
                                | ValueSome solution -> solution
                                | ValueNone -> document.Project.Solution
                        }

                    let action =
                        CodeAction.Create(
                            title,
                            Func<CancellationToken, Task<Solution>>(fun cancellationToken ->
                                CancellableTask.start cancellationToken changedSolution),
                            title
                        )

                    context.RegisterRefactoring action
                | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
