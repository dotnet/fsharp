module FSharp.Editor.Tests.Refactors.AsyncBugReproduction

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
[<InlineData("(a:float) (b:int)", "float")>]
let ``Reproducing code null spanStart Bug`` (functionHeader: string) (returnType: string) =
    task {

        let symbolName = "sum"

        let code =
            $"""
            let sum {functionHeader}= a + b
            """

        use context = TestContext.CreateWithCode code

        let spanStart = code.IndexOf(symbolName)
        let! newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())
        Console.WriteLine("Test")
    }

[<Theory>]
[<InlineData("(a:float) (b:int)", "float")>]
let ``But works when not async`` (functionHeader: string) (returnType: string) =

    let symbolName = "sum"

    let code =
        $"""
        let sum {functionHeader}= a + b
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf(symbolName)

    let newDocTask = tryRefactor code spanStart context (new AddExplicitReturnType())
    let newDoc = newDocTask.GetAwaiter().GetResult()

    Console.WriteLine("Test")
