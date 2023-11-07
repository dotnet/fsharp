module FSharp.Editor.Tests.Refactors.RefactorTestFramework

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Diagnostics
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CodeActions
open System.Collections.Generic
open System.Threading
open Microsoft.CodeAnalysis.Tags
open System.Reflection
open Microsoft.FSharp.Reflection
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open NUnit.Framework

let GetTaskResult (task: Tasks.Task<'T>) = task.GetAwaiter().GetResult()

let GetSymbol (symbolName: string) (document: Document) ct =
    task {
        let! (_, checkFileResults) = document.GetFSharpParseAndCheckResultsAsync "" |> CancellableTask.start ct

        let symbols = checkFileResults.GetAllUsesOfAllSymbolsInFile ct
        let symbolUse = symbols |> Seq.find (fun s -> s.Symbol.DisplayName = symbolName)

        return
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as v -> Some(v)
            | _ -> None

    }

let GetReturnTypeOfSymbol (symbolName: string) (document: Document) ct =
    let (_, checkFileResults) =
        document.GetFSharpParseAndCheckResultsAsync ""
        |> CancellableTask.start ct
        |> GetTaskResult

    let symbols = checkFileResults.GetAllUsesOfAllSymbolsInFile ct |> List.ofSeq
    let symbolUse = symbols |> Seq.find (fun s -> s.Symbol.DisplayName = symbolName)

    match symbolUse.Symbol with
    | :? FSharpMemberOrFunctionOrValue as v -> Some(v.ReturnParameter.Type.TypeDefinition.CompiledName)
    | _ -> None

let TryGetRangeOfExplicitReturnType (symbolName: string) (document: Document) ct =
    task {
        let! parseFileResults = document.GetFSharpParseResultsAsync symbolName ct
        let! symbol = GetSymbol symbolName document ct

        let range =
            symbol
            |> Option.bind (fun sym -> parseFileResults.RangeOfReturnTypeDefinition(sym.DeclarationLocation.Start, false))

        return range
    }

let AssertCodeHasNotChanged (code: string) (document: Document) ct =
    task {
        let! newText = document.GetTextAsync ct
        Assert.AreEqual(code, newText.ToString())
    }

let AssertHasAnyExplicitReturnType (symbolName: string) (document: Document) ct =
    task {
        let! range = TryGetRangeOfExplicitReturnType symbolName document ct
        Assert.IsTrue(range.IsSome)
    }

let AssertHasSpecificExplicitReturnType (symbolName: string) (expectedTypeName: string) (document: Document) ct =

    let returnType = GetReturnTypeOfSymbol symbolName document ct

    match returnType with
    | Some t -> Assert.AreEqual(expectedTypeName, t)
    | None -> Assert.Fail($"Unexpected type. Expected {expectedTypeName} but was t")

    ()

let AssertHasNoExplicitReturnType (symbolName: string) (document: Document) ct =
    task {
        let! parseFileResults = document.GetFSharpParseResultsAsync symbolName ct
        let! symbol = GetSymbol symbolName document ct

        let range =
            symbol
            |> Option.bind (fun sym -> parseFileResults.TryRangeOfReturnTypeHint(sym.DeclarationLocation.Start, false))

        Assert.IsTrue(range.IsSome)
    }

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

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let tryRefactor (code: string) (cursorPosition) (context: TestContext) (refactorProvider: 'T :> CodeRefactoringProvider) =
    let refactoringActions = new List<CodeAction>()
    let existingDocument = RoslynTestHelpers.GetSingleDocument context.Solution

    context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

    let document = RoslynTestHelpers.GetSingleDocument context.Solution

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
        let existingDocument = RoslynTestHelpers.GetSingleDocument context.Solution

        context.Solution <- context.Solution.WithDocumentText(existingDocument.Id, SourceText.From(code))

        let document = RoslynTestHelpers.GetSingleDocument context.Solution

        let mutable workspace = context.Solution.Workspace

        let refactoringContext =
            CodeRefactoringContext(document, TextSpan(cursorPosition, 1), (fun a -> refactoringActions.Add a), context.CT)

        do! refactorProvider.ComputeRefactoringsAsync refactoringContext

        return refactoringActions
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result
