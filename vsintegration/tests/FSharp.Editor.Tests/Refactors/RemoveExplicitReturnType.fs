module FSharp.Editor.Tests.Refactors.RemoveExplicitReturnType

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open NUnit.Framework
open FSharp.Editor.Tests.Refactors.RefactorTestFramework
open Microsoft.CodeAnalysis.Text
open Xunit
open System.Runtime.InteropServices

[<Theory>]
[<InlineData(":int")>]
[<InlineData(":System.float")>]
let ``Removes explicit return type`` (toRemove: string) =
    task {
        let symbolName = "sum"

        let code =
            $"""
            let sum a b {toRemove}= a + b
            """

        use context = TestContext.CreateWithCode code
        let spanStart = code.IndexOf symbolName

        let! newDoc = tryRefactor code spanStart context (new RemoveExplicitReturnType())

        do! AssertHasNoExplicitReturnType symbolName newDoc context.CT
    }

[<Theory>]
[<InlineData(" :int")>]
[<InlineData(" : int")>]
[<InlineData(" :    int")>]
let ``Empty Space doesnt matter`` (toRemove: string) =
    task {
        let symbolName = "sum"

        let code =
            $"""
            let sum a b {toRemove}= a + b
            """

        use context = TestContext.CreateWithCode code
        let spanStart = code.IndexOf symbolName

        let! newDoc = tryRefactor code spanStart context (new RemoveExplicitReturnType())

        do! AssertHasNoExplicitReturnType symbolName newDoc context.CT
    }

[<Theory>]
[<InlineData("(a:int) (b:int) :int")>]
[<InlineData("(a:System.float) (b:int) :System.float")>]
let ``Different Formatting`` (functionHeader: string) =
    task {
        let symbolName = "sum"

        let code =
            $"""
            let sum {functionHeader}=
                a + b
            """

        use context = TestContext.CreateWithCode code
        let spanStart = code.IndexOf symbolName

        let! newDoc = tryRefactor code spanStart context (new RemoveExplicitReturnType())

        do! AssertHasNoExplicitReturnType symbolName newDoc context.CT
    }

[<Fact>]
let ``Refactor should not be suggested if theres nothing to remove`` () =
    task {

        let symbolName = "sum"

        let code =
            $"""
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! actions = tryGetRefactoringActions code spanStart context (new RemoveExplicitReturnType())

        do Assert.Empty(actions)
    }

[<Fact>]
let ``Refactor should not be suggested if it changes the return type`` () =
    task {
        let symbolName = "sum"

        let code =
            """
            type A = { X:int }
            type B = { X:int }
            
            let f (i: int) : A = { X = i }
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! actions = tryGetRefactoringActions code spanStart context (new RemoveExplicitReturnType())

        do Assert.Empty(actions)
    }

[<Fact>]
let ``Refactor should be suggested if it does not changes the return type`` () =
    task {
        let symbolName = "sum"

        let code =
            """
            
            type B = { X:int }
            type A = { X:int }
            
            let f (i: int) : A = { X = i }
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! actions = tryGetRefactoringActions code spanStart context (new RemoveExplicitReturnType())

        do Assert.NotEmpty(actions)
    }
