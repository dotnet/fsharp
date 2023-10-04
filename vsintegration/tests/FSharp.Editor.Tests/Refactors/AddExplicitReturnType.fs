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


[<Fact>]
let ``Refactor changes something`` () =
    task {
        let code =
            """
            let sum a b = a + b
            """

        let! ct = Async.CancellationToken

        let sourceText = SourceText.From code
        let document = RoslynTestHelpers.GetFsDocument code
        let spanStart = code.IndexOf "sum"
        let span = TextSpan(spanStart, 3)

        let mutable refactorContext =
            CodeRefactoringContext(document, span, Action<CodeActions.CodeAction>(fun a -> ()), ct)

        let refactorProvider = AddExplicitReturnType()
        let expected = None

        do! refactorProvider.ComputeRefactoringsAsync refactorContext
        let newText = match refactorContext.TextDocument.TryGetText() with
            | true,result -> result
            | false,_ -> sourceText

        let! text = refactorContext.TextDocument.GetTextAsync( ct)
        Assert.AreNotEqual(sourceText.ToString(),newText.ToString(),"")

        ()
    }
