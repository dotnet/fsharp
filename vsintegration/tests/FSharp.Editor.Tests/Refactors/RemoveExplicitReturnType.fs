module FSharp.Editor.Tests.Refactors.RemoveExplicitReturnType

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
open FSharp.Editor.Tests.Refactors.RefactorTestFramework
open Microsoft.Build.Utilities
open System.Threading
open FSharp.Test.ReflectionHelper
open Microsoft.Build.Utilities
open FSharp.Test.ProjectGeneration.ProjectOperations
open FSharp.Compiler.Symbols

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
