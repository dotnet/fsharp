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
open FSharp.Editor.Tests.Refactors.RefactorTestFramework
open Microsoft.Build.Utilities
open System.Threading
open FSharp.Test.ReflectionHelper
open Microsoft.Build.Utilities
open FSharp.Test.ProjectGeneration.ProjectOperations
open FSharp.Compiler.Symbols
open Xunit
open System.Runtime.InteropServices

[<Theory>]
[<InlineData(":int")>]
[<InlineData(" :int")>]
[<InlineData(" : int")>]
[<InlineData(" :    int")>]
let ``Refactor should not trigger`` (shouldNotTrigger: string) =
    task {
        let symbolName = "sum"

        let code =
            $"""
            let sum a b {shouldNotTrigger}= a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! actions = tryGetRefactoringActions code spanStart context (new AddExplicitReturnType())

        do Assert.Empty(actions)
    }

[<Fact>]
let ``Correctly infer int as explicit return type`` () =
    task {

        let symbolName = "sum"

        let code =
            """
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

        do! AssertHasSpecificExplicitReturnType symbolName "int" newDoc context.CT
    }

[<Fact>]
let ``Correctly infer custom type that is declared earlier in file`` () =
    task {
        let symbolName = "sum"

        let code =
            """
            type MyType = { Value: int }
            let sum a b = {Value=a+b}
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf symbolName

        let! newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

        do! AssertHasSpecificExplicitReturnType symbolName "MyType" newDoc context.CT

    }
