module FSharp.Editor.Tests.Refactors.RefactorTestFramework

open System
open System.Linq
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

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
        let mutable solution = RoslynTestHelpers.CreateSolution(codeForPreviousFile)

        let firstProject = solution.Projects.First()
        solution <- solution.AddDocument(DocumentId.CreateNewId(firstProject.Id), "test2.fs", code, filePath = "C:\\test2.fs")

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
