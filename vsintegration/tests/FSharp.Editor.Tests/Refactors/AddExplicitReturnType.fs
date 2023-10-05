module FSharp.Editor.Tests.Refactors.AddExplicitReturnType

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open System
open System.Collections.Immutable
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Diagnostics
open FSharp.Editor.Tests.Helpers

open Microsoft.CodeAnalysis.CodeRefactorings
open NUnit.Framework
open Microsoft.CodeAnalysis.CodeActions
open System.Collections.Generic
open Microsoft.VisualStudio.LanguageServices


[<Fact>]
let ``Refactor changes something`` () =
    task {
        let code =
            """
            let sum a b = a + b
            """

        let! ct = Async.CancellationToken

        let solution = RoslynTestHelpers.CreateSolution(code)
        let workspace = solution.Workspace
        let document = RoslynTestHelpers.GetSingleDocument solution

        let spanStart = code.IndexOf "sum"
        let span = TextSpan(spanStart, 3)
        
        
        let refactorProvider = AddExplicitReturnType()
        let refactoringActions= new List<CodeAction>()

        let mutable refactorContext =
            CodeRefactoringContext(document, span, (fun a -> refactoringActions.Add (a)), ct)


        do! refactorProvider.ComputeRefactoringsAsync refactorContext

        let! operations = refactoringActions[0].GetOperationsAsync(ct)

        for operation in operations do
            operation.Apply (workspace,ct)


        
        let! refactorText = refactorContext.TextDocument.GetTextAsync ct
        refactorText
        let! text = document.GetTextAsync ct
        solution.Id
        Assert.AreNotEqual(code,text.ToString(),"")

        ()
    }
