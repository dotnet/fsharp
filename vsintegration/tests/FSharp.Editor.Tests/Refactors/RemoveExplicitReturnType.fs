module FSharp.Editor.Tests.Refactors.RemoveExplicitReturnType

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open NUnit.Framework
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

[<Fact>]
let ``Refactor changes something`` () =
    task {

        let code =
            $"""
            let sum a b :int= a + b
            """

        use context = TestContext.CreateWithCode code
        let spanStart = code.IndexOf "sum"

        let! (_, text) = tryRefactor code spanStart context (new RemoveExplicitReturnType())

        Assert.AreNotEqual(code, text.ToString(), "")

        ()
    }

[<Fact>]
let ``Refactor changes nothing`` () =
    task {

        let code =
            $"""
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf "sum"

        let! (_, text) = tryRefactor code spanStart context (new RemoveExplicitReturnType())

        Assert.AreEqual(code, text.ToString(), "")

        ()
    }