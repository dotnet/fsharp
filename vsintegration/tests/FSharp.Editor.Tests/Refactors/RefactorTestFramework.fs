module FSharp.Editor.Tests.Refactors.RefactorTestFramework

open System
open System.Linq
open System.Collections.Immutable
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open System.Threading
open FSharp.Compiler.Text

let GetTaskResult (task: Tasks.Task<'T>) = task.GetAwaiter().GetResult()

type TestCodeFix = { Message: string; FixedCode: string }

type TestContext(Solution: Solution, CT) =
    let mutable _solution = Solution
    member this.CT = CT

    member this.Solution
        with set value = _solution <- value
        and get () = _solution

    interface IDisposable with
        member this.Dispose() = Solution.Workspace.Dispose()

    static member CreateWithCode(code: string) =
        let solution = RoslynTestHelpers.CreateSolution(code)
        let ct = CancellationToken false
        new TestContext(solution, ct)

    static member CreateWithCodeAndDependency (code: string) (codeForPreviousFile: string) =
        let mutable solution = RoslynTestHelpers.CreateSolution(codeForPreviousFile)

        let firstProject = solution.Projects.First()
        solution <- solution.AddDocument(DocumentId.CreateNewId(firstProject.Id), "test2.fs", code, filePath = "C:\\test2.fs")

        let ct = CancellationToken false
        new TestContext(solution, ct)

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let tryRefactor (code: string) (cursorPosition) (context: TestContext) (refactorProvider: 'T :> CodeRefactoringProvider) =
    let refactoringActions = new List<CodeAction>()
    let existingDocument = RoslynTestHelpers.GetLastDocument context.Solution

    context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

    let document = RoslynTestHelpers.GetLastDocument context.Solution

    let mutable workspace = context.Solution.Workspace

    let refactoringContext =
        CodeRefactoringContext(document, TextSpan(cursorPosition, 1), (fun a -> refactoringActions.Add a), context.CT)

    let task = refactorProvider.ComputeRefactoringsAsync refactoringContext
    do task.GetAwaiter().GetResult()

    for action in refactoringActions do
        let operationsTask = action.GetOperationsAsync context.CT
        let operations = operationsTask |> GetTaskResult

        for operation in operations do
            let codeChangeOperation = operation :?> ApplyChangesOperation
            codeChangeOperation.Apply(workspace, context.CT)
            context.Solution <- codeChangeOperation.ChangedSolution
            ()

    let newDocument = context.Solution.GetDocument(document.Id)
    newDocument

let tryGetRefactoringActions (code: string) (cursorPosition) (context: TestContext) (refactorProvider: 'T :> CodeRefactoringProvider) =
    cancellableTask {
        let refactoringActions = new List<CodeAction>()
        let existingDocument = RoslynTestHelpers.GetLastDocument context.Solution

        context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

        let document = RoslynTestHelpers.GetLastDocument context.Solution

        let mutable workspace = context.Solution.Workspace

        let refactoringContext =
            CodeRefactoringContext(document, TextSpan(cursorPosition, 1), (fun a -> refactoringActions.Add a), context.CT)

        do! refactorProvider.ComputeRefactoringsAsync refactoringContext

        return refactoringActions
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result
