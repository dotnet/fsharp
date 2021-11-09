// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests

open Xunit
open FSharp.Test.Compiler

module CastingTests =

    [<Fact>]
    let ``Compile: ValueTuple.Create(1,1) :> IComparable<ValueTuple<int,int>>`` () =
        FSharp """
module One
open System

let y = ValueTuple.Create(1,1) :> IComparable<ValueTuple<int,int>>
printfn "%A" y
    """
     |> ignoreWarnings
     |> compile
     |> shouldSucceed

    [<Fact>]
    let ``Compile: struct (1,2) :> IComparable<ValueTuple<int,int>>`` () =
        FSharp """
module One
open System

let y = struct (1,2) :> IComparable<ValueTuple<int,int>>
printfn "%A" y
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Compile: let x = struct (1,3); x :> IComparable<ValueTuple<int,int>>`` () =
        FSharp """
module One
open System

let y = struct (1,3) :> IComparable<ValueTuple<int,int>>
printfn "%A" y
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Script: ValueTuple.Create(0,0) :> IComparable<ValueTuple<int,int>>`` () =
        Fsx """
module One
open System

let y = ValueTuple.Create(1,1) :> IComparable<ValueTuple<int,int>>
printfn "%A" y
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Script: struct (1,2) :> IComparable<ValueTuple<int,int>>`` () =
        Fsx """
module One
open System

let y = struct (0,0) :> IComparable<ValueTuple<int,int>>
printfn "%A" y
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Script: let x = struct (1,3); x :> IComparable<ValueTuple<int,int>>`` () =
        Fsx """
module One
open System

let x = struct (1,3)
let y = x :> IComparable<ValueTuple<int,int>>
printfn "%A" y
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Compile: (box (System.ValueTuple.Create(1,2)) :?> System.IComparable<System.ValueTuple<int,int>>)`` () =
        FSharp """
module One
open System

let y = (box (System.ValueTuple.Create(1,2)) :?> System.IComparable<System.ValueTuple<int,int>>)
printfn "%A" y
    """
     |> ignoreWarnings
     |> compile
     |> shouldSucceed

    [<Fact>]
    let ``Script: (box (System.ValueTuple.Create(1,2)) :?> System.IComparable<System.ValueTuple<int,int>>)`` () =
        FSharp """
module One
open System

let y = (box (System.ValueTuple.Create(1,2)) :?> System.IComparable<System.ValueTuple<int,int>>)
printfn "%A" y
    """
     |> ignoreWarnings
     |> compile
     |> shouldSucceed
