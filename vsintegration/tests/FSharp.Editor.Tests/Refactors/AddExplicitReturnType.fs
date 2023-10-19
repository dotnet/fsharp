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

[<Fact>]
let ``Refactor changes something`` () =
    task {

        let code =
            """
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf "sum"

        let! (_, text) = tryRefactor code spanStart context (new AddExplicitReturnType())

        Assert.AreNotEqual(code, text.ToString(), "")

        ()
    }

[<Fact>]
let ``Refactor changes nothing`` () =
    task {

        let code =
            """
            let sum a b :int= a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf "sum"

        let! (_, text) = tryRefactor code spanStart context (new AddExplicitReturnType())

        Assert.AreEqual(code, text.ToString(), "")

        ()
    }

[<Fact>]
let ``Correctly infer int as explicit return type`` () =
    task {

        let code =
            """
            let sum a b = a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf "sum"

        let! (document, text) = tryRefactor code spanStart context (new AddExplicitReturnType())

        let! (parseFileResults, checkFileResults) =
            document.GetFSharpParseAndCheckResultsAsync "test"
            |> CancellableTask.start context.CT

        let symbols = checkFileResults.GetAllUsesOfAllSymbolsInFile context.CT
        let symbolUse = symbols |> Seq.find (fun s -> s.Symbol.DisplayName = "sum")

        match symbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> Assert.AreEqual("int", v.ReturnParameter.Type.TypeDefinition.CompiledName)
        | _ -> failwith "Unexpected symbol"

        ()
    }
