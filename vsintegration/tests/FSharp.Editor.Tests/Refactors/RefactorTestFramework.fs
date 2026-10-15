module FSharp.Editor.Tests.Refactors.RefactorTestFramework

open System
open System.Linq
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Compiler.Diagnostics
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

let caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let textOf (document: Document) =
    (document.GetTextAsync() |> GetTaskResult).ToString()

let errorsOf (document: Document) =
    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    checkResults.Diagnostics
    |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

let private moduleHeader (body: string) =
    let lineBreak = body[.. body.IndexOf('\n')]
    lineBreak + "module M" + lineBreak

let inModuleM (body: string) = moduleHeader body + body

let refactorAt (provider: 'T :> CodeRefactoringProvider) (context: TestContext) (code: string) (marker: string) =
    tryRefactor code (caretAt code marker) context provider

let refactoredBy (provider: 'T :> CodeRefactoringProvider) (body: string) (marker: string) =
    let code = inModuleM body
    use context = TestContext.CreateWithCode code
    let document = refactorAt provider context code marker
    Assert.Empty(errorsOf document)

    let header = moduleHeader body
    let text = textOf document
    Assert.StartsWith(header, text, StringComparison.Ordinal)
    text.Substring header.Length

let convertsBothWaysBy (provider: 'T :> CodeRefactoringProvider) (markers: string seq) (reference: string) (structs: string) =
    for marker in markers do
        Assert.Equal(structs, refactoredBy provider reference marker)
        Assert.Equal(reference, refactoredBy provider structs marker)

let actionsBy (provider: 'T :> CodeRefactoringProvider) (body: string) (marker: string) =
    let code = inModuleM body
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context provider

// The checker reads the other file of a synthetic project from disk, so the result is checked as a new project.
let refactoredAcrossFiles (provider: 'T :> CodeRefactoringProvider) (dependency: string) (code: string) (marker: string) =
    use context = TestContext.CreateWithCodeAndDependency code dependency
    let document = refactorAt provider context code marker

    let dependencyAfter =
        (context.Solution.Projects |> Seq.head).Documents |> Seq.head |> textOf

    use checkContext =
        TestContext.CreateWithCodeAndDependency (textOf document) dependencyAfter

    Assert.Empty((checkContext.Solution.Projects |> Seq.head).Documents |> Seq.last |> errorsOf)

    struct (textOf document, dependencyAfter)
