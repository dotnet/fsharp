// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.Threading.Tasks
open FSharp.Test.ScriptHelpers
open Xunit

type CompletionTests() =

    [<Fact>]
    member _.``Instance completions in the same submission``() =
        async {
            use script = new FSharpScript()
            let lines = [ "let x = 1"
                          "x." ]
            let! completions = script.GetCompletionItems(String.Join("\n", lines), 2, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "CompareTo")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Instance completions from a previous submission``() =
        async {
            use script = new FSharpScript()
            script.Eval("let x = 1") |> ignoreValue
            let! completions = script.GetCompletionItems("x.", 1, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "CompareTo")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Completions from types that try to pull in Windows runtime extensions``() =
        async {
            use script = new FSharpScript()
            script.Eval("open System") |> ignoreValue
            script.Eval("let t = TimeSpan.FromHours(1.0)") |> ignoreValue
            let! completions = script.GetCompletionItems("t.", 1, 2)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "TotalHours")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Static member completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.String.", 1, 14)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "Join")
            Assert.True(matchingCompletions.Length >= 1)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Type completions from namespace``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.", 1, 7)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "String")
            Assert.True(matchingCompletions.Length >= 1)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Namespace completions``() =
        async {
            use script = new FSharpScript()
            let! completions = script.GetCompletionItems("System.", 1, 7)
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "Collections")
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
            let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "Select")
            Assert.Equal(1, matchingCompletions.Length)
        } |> Async.StartAsTask :> Task

    [<Fact>]
    member _.``Completions with strange names for static members``() =
        use script = new FSharpScript()
        let lines = [ "type C() ="
                      "    static member ``Long Name`` = 1"
                      "    static member ``-``(a:C, b:C) = C()"
                      "    static member op_Addition(a:C, b:C) = C()"
                      "    static member ``base``(a:C, b:C) = C()"
                      "    static member ``|A|_|``(a:C, b:C) = C()"
                      "C." ]
        let completions = script.GetCompletionItems(String.Join("\n", lines), 7, 2) |> Async.RunSynchronously
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "Long Name")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``Long Name``", matchingCompletions.[0].NameInCode)

        // Things with names like op_Addition are suppressed from completion by FCS
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "+")
        Assert.Equal(0, matchingCompletions.Length)

        // Strange names like ``+`` and ``-`` are just normal text and not operators
        // and are present in competion lists
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "-")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``-``", matchingCompletions.[0].NameInCode)

        // ``base`` is a strange name but is still present. In this case the inserted
        // text NameInCode is ``base`` because the thing is not a base value mapping to
        // the base keyword
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "base")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``base``", matchingCompletions.[0].NameInCode)

        // ``|A|_|`` is a strange name like the name of an active pattern but is still present. 
        // In this case the inserted is (|A|_|)
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "|A|_|")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("(|A|_|)", matchingCompletions.[0].NameInCode)


    [<Fact>]
    member _.``Completions with strange names for module``() =
        use script = new FSharpScript()
        let lines = [ "module M ="
                      "    let ``Long Name`` = 1"
                      "    let ``-``(a:int, b:int) = 1"
                      "    let op_Addition(a:int, b:int) = 1"
                      "    let ``base``(a:int, b:int) = 1"
                      "    let (|A|_|)(a:int, b:int) = None"
                      "M." ]
        let completions = script.GetCompletionItems(String.Join("\n", lines), 7, 2) |> Async.RunSynchronously
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "Long Name")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``Long Name``", matchingCompletions.[0].NameInCode)

        // Things with names like op_Addition are suppressed from completion by FCS
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "+")
        Assert.Equal(0, matchingCompletions.Length)

        // Strange names like ``+`` and ``-`` are just normal text and not operators
        // and are present in competion lists
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "-")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``-``", matchingCompletions.[0].NameInCode)

        // ``base`` is a strange name but is still present. In this case the inserted
        // text NameInCode is ``base`` because the thing is not a base value mapping to
        // the base keyword
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "base")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``base``", matchingCompletions.[0].NameInCode)

        // (|A|_|) is an active pattern and the completion item is the
        // active pattern case A. In this case the inserted is A
        let matchingCompletions = completions |> Array.filter (fun d -> d.NameInList = "A")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("A", matchingCompletions.[0].NameInCode)


