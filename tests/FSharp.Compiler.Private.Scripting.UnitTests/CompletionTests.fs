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
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "Long Name")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``Long Name``", matchingCompletions.[0].NameInCode)

        // Things with names like op_Addition are suppressed from completion by FCS
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "+")
        Assert.Equal(0, matchingCompletions.Length)

        // Strange names like ``+`` and ``-`` are just normal text and not operators
        // and are present in competion lists
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "-")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``-``", matchingCompletions.[0].NameInCode)

        // ``base`` is a strange name but is still present. In this case the inserted
        // text NameInCode is ``base`` because the thing is not a base value mapping to
        // the base keyword
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "base")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``base``", matchingCompletions.[0].NameInCode)

        // ``|A|_|`` is a strange name like the name of an active pattern but is still present. 
        // In this case the inserted is (|A|_|)
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "|A|_|")
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
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "Long Name")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``Long Name``", matchingCompletions.[0].NameInCode)

        // Things with names like op_Addition are suppressed from completion by FCS
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "+")
        Assert.Equal(0, matchingCompletions.Length)

        // Strange names like ``+`` and ``-`` are just normal text and not operators
        // and are present in competion lists
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "-")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``-``", matchingCompletions.[0].NameInCode)

        // ``base`` is a strange name but is still present. In this case the inserted
        // text NameInCode is ``base`` because the thing is not a base value mapping to
        // the base keyword
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "base")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("``base``", matchingCompletions.[0].NameInCode)

        // (|A|_|) is an active pattern and the completion item is the
        // active pattern case A. In this case the inserted is A
        let matchingCompletions = completions |> Array.filter (fun d -> d.Name = "A")
        Assert.Equal(1, matchingCompletions.Length)
        Assert.Equal("A", matchingCompletions.[0].NameInCode)

    [<Fact>]
    member _.``Completions in a match clause prepend union name to case when qualified access is required``() =
        use script = new FSharpScript()
        let text = """
module M =
    [<RequireQualifiedAccess>]
    type ChoiceZ =
        | Choice1
        | Choice2

open M

let call (choice: M.ChoiceZ) =
    match choice with
    | C
        """
        let completions = script.GetCompletionItems(text, 12, 7) |> Async.RunSynchronously
        
        Assert.Equal (2, completions.Length)

        for c in completions do
            Assert.Equal (None, c.NamespaceToOpen)
            Assert.Equal ("ChoiceZ." + c.Name, c.NameInCode)

    [<Fact>]
    member _.``Completions in a match clause prepend union name to case and open modules when qualified access is required and union is out of scope``() =
        use script = new FSharpScript()
        let text = """
module M =
    [<RequireQualifiedAccess>]
    type ChoiceZ =
        | Choice1
        | Choice2

let call (choice: M.ChoiceZ) =
    match choice with
    | C
        """
        let completions = script.GetCompletionItems(text, 10, 7) |> Async.RunSynchronously
        
        Assert.Equal (2, completions.Length)

        for c in completions do
            Assert.Equal (Some "M", c.NamespaceToOpen)
            Assert.StartsWith ("ChoiceZ.", c.NameInCode)

    [<Fact>]
    member _.``Completions in a match clause do not unnecessarily open an AutoOpen module``() =
        use script = new FSharpScript()
        let text = """
[<AutoOpen>]
module M =
    type ChoiceZ =
        | Choice1
        | Choice2

let call (choice: M.ChoiceZ) =
    match choice with
    | C
        """
        let completions = script.GetCompletionItems(text, 10, 7) |> Async.RunSynchronously
        
        Assert.Equal (2, completions.Length)

        for c in completions do
            Assert.Equal (None, c.NamespaceToOpen)
            Assert.Equal (c.Name, c.NameInCode)

    [<Fact>]
    member _.``Completions in a match clause open the correct modules``() =
        use script = new FSharpScript()
        let text = """
module F =
    module N =
        [<AutoOpen>]
        module M =
            type ChoiceZ =
                | Choice1
                | Choice2

    let call (choice: N.M.ChoiceZ) =
        match choice with
        | C
        """
        let completions = script.GetCompletionItems(text, 12, 11) |> Async.RunSynchronously
        
        Assert.Equal (2, completions.Length)

        for c in completions do
            Assert.Equal (Some "N.M", c.NamespaceToOpen)
            Assert.Equal (c.Name, c.NameInCode)

