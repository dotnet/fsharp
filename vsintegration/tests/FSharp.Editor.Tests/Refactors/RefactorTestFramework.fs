module FSharp.Editor.Tests.Refactors.RefactorTestFramework

open System
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Test.ProjectGeneration

open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open System.Threading

let GetTaskResult (task: Tasks.Task<'T>) = task.GetAwaiter().GetResult()

type TestContext(Solution: Solution) =
    let mutable _solution = Solution
    member _.CancellationToken = CancellationToken.None

    member _.Solution
        with set value = _solution <- value
        and get () = _solution

    interface IDisposable with
        member _.Dispose() = Solution.Workspace.Dispose()

    static member CreateWithCode(code: string) =
        let solution = RoslynTestHelpers.CreateSolution(code)
        new TestContext(solution)

    static member CreateWithCodeAndDependency (code: string) (codeForPreviousFile: string) =
        let project =
            { SyntheticProject.Create(
                  { sourceFile "First" [] with
                      Source = codeForPreviousFile
                  },
                  { sourceFile "Second" [ "First" ] with
                      Source = code
                  }
              ) with
                AutoAddModules = false
            }

        let solution, _ = RoslynTestHelpers.CreateSolution project
        new TestContext(solution)

let tryRefactor (code: string) (cursorPosition) (context: TestContext) (refactorProvider: 'T :> CodeRefactoringProvider) =
    cancellableTask {
        let mutable action: CodeAction = null
        let existingDocument = RoslynTestHelpers.GetLastDocument context.Solution

        context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

        let document = RoslynTestHelpers.GetLastDocument context.Solution

        let mutable workspace = context.Solution.Workspace

        let refactoringContext =
            CodeRefactoringContext(document, TextSpan(cursorPosition, 1), (fun a -> action <- a), context.CancellationToken)

        do! refactorProvider.ComputeRefactoringsAsync refactoringContext

        let! operations = action.GetOperationsAsync context.CancellationToken

        for operation in operations do
            let codeChangeOperation = operation :?> ApplyChangesOperation
            codeChangeOperation.Apply(workspace, context.CancellationToken)
            context.Solution <- codeChangeOperation.ChangedSolution
            ()

        let newDocument = context.Solution.GetDocument(document.Id)
        return newDocument

    }
    |> CancellableTask.startWithoutCancellation
    |> GetTaskResult

let tryGetRefactoringActions (code: string) (cursorPosition) (context: TestContext) (refactorProvider: 'T :> CodeRefactoringProvider) =
    cancellableTask {
        let refactoringActions = new List<CodeAction>()
        let existingDocument = RoslynTestHelpers.GetLastDocument context.Solution

        context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

        let document = RoslynTestHelpers.GetLastDocument context.Solution

        let refactoringContext =
            CodeRefactoringContext(document, TextSpan(cursorPosition, 1), (fun a -> refactoringActions.Add a), context.CancellationToken)

        do! refactorProvider.ComputeRefactoringsAsync refactoringContext

        return refactoringActions
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result

let tryGetRefactoringActionsForSpan (code: string) (span: TextSpan) (context: TestContext) (refactorProvider: CodeRefactoringProvider) =
    let actions = List<CodeAction>()
    let existingDocument = RoslynTestHelpers.GetLastDocument context.Solution
    context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))
    let document = RoslynTestHelpers.GetLastDocument context.Solution

    let refactoringContext =
        CodeRefactoringContext(document, span, (fun action -> actions.Add action), context.CancellationToken)

    refactorProvider.ComputeRefactoringsAsync(refactoringContext).GetAwaiter().GetResult()
    actions

let refactorSpan (code: string) (span: TextSpan) (title: string) (context: TestContext) (refactorProvider: CodeRefactoringProvider) =
    let action =
        tryGetRefactoringActionsForSpan code span context refactorProvider
        |> Seq.find (fun action -> String.Equals(action.Title, title, StringComparison.Ordinal))

    for operation in action.GetOperationsAsync(context.CancellationToken) |> GetTaskResult do
        let applyChanges = operation :?> ApplyChangesOperation
        applyChanges.Apply(context.Solution.Workspace, context.CancellationToken)
        context.Solution <- applyChanges.ChangedSolution

    RoslynTestHelpers.GetLastDocument context.Solution

let selectionOf (code: string) (selected: string) =
    TextSpan(code.IndexOf(selected, StringComparison.Ordinal), selected.Length)

let caretAt (code: string) (marker: string) =
    let offset = max 0 (marker.IndexOf '|')
    TextSpan(code.IndexOf(marker.Replace("|", ""), StringComparison.Ordinal) + offset, 0)

let fileOf (declarations: string) =
    let text = declarations.Trim('\r', '\n')

    if
        text.StartsWith("module ", StringComparison.Ordinal)
        || text.StartsWith("namespace ", StringComparison.Ordinal)
    then
        declarations
    else
        let lineBreak = if declarations.IndexOf '\r' >= 0 then "\r\n" else "\n"
        $"{lineBreak}module M{lineBreak}{lineBreak}{text}{lineBreak}"

let refactoredText (provider: CodeRefactoringProvider) (context: TestContext) (title: string) (code: string) (span: TextSpan) =
    let document = refactorSpan code span title context provider

    let parseResults =
        document.GetFSharpParseResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(parseResults.Diagnostics)
    (document.GetTextAsync() |> GetTaskResult).ToString()

let offeredTitles (provider: CodeRefactoringProvider) (context: TestContext) (code: string) (span: TextSpan) =
    tryGetRefactoringActionsForSpan code span context provider
    |> Seq.map _.Title
    |> String.concat ";"
