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
    let a:(Action<ReadOnlySpan<int>>) = fun _ -> ()
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
    let ``Ref structs in generics - GetAlternateLookup`` () =
        FSharp """module Foo
open System
open System.Collections.Generic

let main _args = 
    let myDict = ["x",1;"y",2] |> dict |> Dictionary 
    let altLookup = myDict.GetAlternateLookup<string,int,ReadOnlySpan<char>>()
    altLookup.ContainsKey(ReadOnlySpan([|'x'|]))
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

