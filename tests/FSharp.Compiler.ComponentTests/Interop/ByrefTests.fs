// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open FSharp.Test
open FSharp.Test.Compiler

module ``Byref interop verification tests`` =

    [<FactForNETCOREAPP>]
    let ``Test that ref readonly is treated as inref`` () =

        FSharp """
        module ByrefTest
        open System.Runtime.CompilerServices
        type MyRecord = { Value : int } with
            member this.SetValue(v: int) = (Unsafe.AsRef<int> &this.Value) <- v

        let check mr =
            if mr.Value <> 1 then
                failwith "Value should be 1"

            mr.SetValue(42)

            if mr.Value <> 42 then
                failwith $"Value should be 42, but is {mr.Value}"
            0

        [<EntryPoint>]
        let main _ =
            let mr = { Value = 1 }
            check mr
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Test that ref readonly is treated as inref for ROS .ctor`` () =
        FSharp """
        module Foo
        open System


        [<EntryPoint>]
        let main _ =
            let mutable bt: int = 42
            let ros = ReadOnlySpan<int>(&bt)

            if ros.Length <> 1 || ros[0] <> 42 then
                failwith "Unexpected result"
            0
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - can declare`` () =
        FSharp """module Foo
open System
let x(a:Action<ReadOnlySpan<int>>) = a.Invoke(ReadOnlySpan([||]))
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - can return as inner`` () =

        FSharp """module Foo
open System
let x() =
    let a:(Action<ReadOnlySpan<int>>) = Unchecked.defaultof<_>
    a
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - can use in object expressions`` () =
        FSharp """module Foo
open System

let main _args = 
    let comparer = 
        { new System.IComparable<ReadOnlySpan<int>>
                with member x.CompareTo(o) = 42 }
    comparer.CompareTo(ReadOnlySpan([||]))
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - can use in foreach`` () =
        FSharp """module Foo
open System

let processSeq (input:seq<ReadOnlySpan<int>>) = 
    for ros in input do
        printfn "%i" (ros.Length)
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - IL and runtime test`` () =
        FSharp """module Foo
open System
open System.Collections.Generic

let myDict = ["x",1;"xyz",2] |> dict |> Dictionary 

let checkIfPresent (input:ReadOnlySpan<char>) = 
    let altLookup = myDict.GetAlternateLookup<ReadOnlySpan<char>>()
    let present = altLookup.ContainsKey(input)
    for c in input do
        printf "%c" c
    printfn ": %A" present

[<EntryPoint>]
let main _args = 
    checkIfPresent(ReadOnlySpan<char>([||]))
    checkIfPresent("x".AsSpan())
    checkIfPresent(ReadOnlySpan([|'x';'y';'z'|]))
    0
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutputContains [|": false";"x: true";"xyz: true"|]
        |> verifyIL 
            ["callvirt   instance valuetype [System.Collections]System.Collections.Generic.Dictionary`2/AlternateLookup`1<!0,!1,!!0> class [System.Collections]System.Collections.Generic.Dictionary`2<string,int32>::GetAlternateLookup<valuetype [runtime]System.ReadOnlySpan`1<char>>()"]

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - GetAlternateLookup`` () =
        FSharp """module Foo
open System
open System.Collections.Generic

let main _args = 
    let myDict = ["x",1;"y",2] |> dict |> Dictionary 
    let altLookup = myDict.GetAlternateLookup<ReadOnlySpan<char>>()
    altLookup.ContainsKey(ReadOnlySpan([|'x'|]))
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Xunit.InlineData("task",".Result")>]
    [<Xunit.InlineData("async"," |> Async.RunSynchronously")>]
    [<TheoryForNETCOREAPP>]
    let ``Ref structs in generics - builders`` (build:string) (getter:string) =
        
        FSharp $$$"""module Foo
open System

let getAction() =        
    {{{build}}} {
        let x = new Action<ReadOnlySpan<int>>(fun ros -> printfn "%i" ros.Length)
        return x
    }

let getBuilderResult() = 
    {{{build}}} {
        let! myAction = getAction()
        myAction.Invoke(ReadOnlySpan([|1|]))
        return myAction
    }

[<EntryPoint>]
let main _args = 
    let myTask = getBuilderResult(){{{getter}}}
    printfn "%O" myTask
    0
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutputContains [|"1";"System.Action`1[System.ReadOnlySpan`1[System.Int32]]"|]
        |> verifyIL  
               [ if build = "task" then 
                    "valuetype [FSharp.Core]Microsoft.FSharp.Control.TaskStateMachineData`1<class [runtime]System.Action`1<valuetype [runtime]System.ReadOnlySpan`1<int32>>>>"
                 else "[FSharp.Core]Microsoft.FSharp.Control.FSharpAsync`1<class [runtime]System.Action`1<valuetype [runtime]System.ReadOnlySpan`1<int32>>>>" ]

    [<FactForNETCOREAPP>]
    let ``Ref structs in generics - negative tests`` () =
        FSharp """module Foo
open System
open System.Collections.Generic

[<NoComparison>]
type MyRecordFullOfWrongStuff<'T> =
    { Value : Span<'T> 
      MyMap : list<Span<int>>
      MyDict: Dictionary<string,byref<char>>
      Nested: Span<MyRecordFullOfWrongStuff<int>> }

let processRecord (recd:MyRecordFullOfWrongStuff<Span<Span<Uri>>>) =
    recd.MyDict.["x"]

        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics 
                [ Error 412, Line 7, Col 7, Line 7, Col 12, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 437, Line 6, Col 6, Line 6, Col 30, "A type would store a byref typed value. This is not permitted by Common IL."
                  Error 412, Line 8, Col 7, Line 8, Col 12, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 412, Line 9, Col 7, Line 9, Col 13, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 412, Line 10, Col 7, Line 10, Col 13, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 3300, Line 12, Col 20, Line 12, Col 24, "The parameter 'recd' has an invalid type 'MyRecordFullOfWrongStuff<Span<Span<Uri>>>'. This is not permitted by the rules of Common IL."
                  Error 412, Line 13, Col 5, Line 13, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 412, Line 13, Col 5, Line 13, Col 16, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."
                  Error 412, Line 13, Col 5, Line 13, Col 9, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL."]
                  
