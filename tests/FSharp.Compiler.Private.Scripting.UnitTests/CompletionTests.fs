// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.Threading.Tasks
open FSharp.Compiler.Scripting
open Xunit

type CompletionTests() =

    [<Fact>]
    member _.``Instance completions in the same submission``() =
        async {
            use script = new FSharpScript()
            let lines = [ "let x = 1"
                          "x." ]
            let! completions = script.GetCompletionItems(String.Join("\n", lines), 2, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "CompareTo")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Instance completions from a previous submission``() =
        async {
            use script = new FSharpScript()
            script.Eval("let x = 1") |> ignoreValue
            let! completions = script.GetCompletionItems("x.", 1, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "CompareTo")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Completions from types that try to pull in Windows runtime extensions``() =
        async {
            use script = new FSharpScript()
            script.Eval("open System") |> ignoreValue
            script.Eval("let t = TimeSpan.FromHours(1.0)") |> ignoreValue
            let! completions = script.GetCompletionItems("t.", 1, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "TotalHours")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Static member completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.String.", 1, 14)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "Join")
            Assert.True(matchingCompletions.Length >= 1)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Type completions from namespace``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.", 1, 7)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "String")
            Assert.True(matchingCompletions.Length >= 1)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Namespace completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.", 1, 7)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "Collections")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Extension method completions``() =
        async {
            use script = new FSharpScript()
            let lines = [ "open System.Linq"
                          "let list = new System.Collections.Generic.List<int>()"
                          "list." ]
            let! completions = script.GetCompletionItems(String.Join("\n", lines), 3, 5)
            let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "Select")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task
