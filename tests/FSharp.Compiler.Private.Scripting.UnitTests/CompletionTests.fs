// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.Threading.Tasks
open FSharp.Compiler.Scripting
open NUnit.Framework

[<TestFixture>]
type CompletionTests() =

    [<Test>]
    member _.``Instance completions in the same submission``() =
        async {
            use script = new FSharpScript()
            let lines = [ "let x = 1"
                          "x." ]
            let! completions = script.GetCompletionSymbols(String.Join("\n", lines), 2, 2)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "CompareTo")
            Assert.AreEqual(1, List.length matchingCompletions)
        } |> Async.StartAsTask :> Task

    [<Test>]
    member _.``Instance completions from a previous submission``() =
        async {
            use script = new FSharpScript()
            script.Eval("let x = 1") |> ignoreValue
            let! completions = script.GetCompletionSymbols("x.", 1, 2)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "CompareTo")
            Assert.AreEqual(1, List.length matchingCompletions)
        } |> Async.StartAsTask :> Task

    [<Test>]
    member _.``Static member completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionSymbols("System.String.", 1, 14)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "Join")
            Assert.GreaterOrEqual(List.length matchingCompletions, 1)
        } |> Async.StartAsTask :> Task

    [<Test>]
    member _.``Type completions from namespace``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionSymbols("System.", 1, 7)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "String")
            Assert.GreaterOrEqual(List.length matchingCompletions, 1)
        } |> Async.StartAsTask :> Task

    [<Test>]
    member _.``Namespace completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionSymbols("System.", 1, 7)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "Collections")
            Assert.AreEqual(1, List.length matchingCompletions)
        } |> Async.StartAsTask :> Task

    [<Test>]
    member _.``Extension method completions``() =
        async {
            use script = new FSharpScript()
            let lines = [ "open System.Linq"
                          "let list = new System.Collections.Generic.List<int>()"
                          "list." ]
            let! completions = script.GetCompletionSymbols(String.Join("\n", lines), 3, 5)
            let matchingCompletions = completions |> List.filter (fun s -> s.DisplayName = "Select")
            Assert.AreEqual(1, List.length matchingCompletions)
        } |> Async.StartAsTask :> Task
