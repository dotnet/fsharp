// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests

open Xunit
open FSharp.Test.Compiler

#if NETCOREAPP
// Test cases for https://github.com/dotnet/fsharp/issues/9369
module IndexerSetterParamArray =

    [<Fact>]
    let ``Indexer setter can use ParamArray`` () =
        FSharp """
module One
open System

type T() =
    let mutable v = ""
    member this.Item
        with get ([<ParamArray>] indices: int[]) = 
            $"{v}; get(%A{indices})"

        and set ([<ParamArray>] indices: int[]) (value: string) =
            v <- $"set(%A{indices}, {value})"

let t = T()

// Using an explicit array is allowed
t[ [| 2 |] ] <- "7" 
let v2 = t[[| 2 |]]
printfn $"v2 = {v2}"
if v2 <> "set([|2|], 7); get([|2|])" then failwith "not right value B"


t[1] <- "7" 
let v3 = t[1]
printfn $"v3 = {v3}"
if v3 <> "set([|1|], 7); get([|1|])" then failwith "not right value C"

t[1, 0] <- "7" 
let v4 = t[1, 0]
printfn $"v4 = {v4}"
if v4 <> "set([|1; 0|], 7); get([|1; 0|])" then failwith "not right value D"

// Inference defaults to the array in absense of other information 
let f idxs =
    t[ idxs ] <- "7" 

f ([| 2 |] )
let v5 = t[ [| 2 |] ]
printfn $"v5 = {v5}"
if v5 <> "set([|2|], 7); get([|2|])" then failwith "not right value"

    """
     |> ignoreWarnings
     |> compileExeAndRun
     |> shouldSucceed


    // In this case the indexers take one initial arg then a ParamArray
    [<Fact>]
    let ``Indexer setter can use ParamArray with one initial arg`` () =
        FSharp """
module One
open System

type T() =
    let mutable v = ""
    member this.Item
        with get (idx1: int) = 
            $"{v}; get({idx1})"
        and set (idx1: int) (value: string) =
            v <- $"set({idx1}, {value})"

    member this.Item
        with get (idx1: int, [<ParamArray>] indices: int[]) = 
            $"{v}; get({idx1}, %A{indices})"

        and set (idx1: int, [<ParamArray>] indices: int[]) (value: string) =
            v <- $"set({idx1}, %A{indices}, {value})"

let t = T()

t[2] <- "7" 
let v1 = t[2]
printfn $"v1 = {v1}"
if v1 <> "set(2, 7); get(2)" then failwith "not right value A"


// Using an explicit array is allowed
t[1, [| 2 |] ] <- "7" 
let v2 = t[1, [| 2 |]]
printfn $"v2 = {v2}"
if v2 <> "set(1, [|2|], 7); get(1, [|2|])" then failwith "not right value B"


t[2, 1] <- "7" 
let v3 = t[2, 1]
printfn $"v3 = {v3}"
if v3 <> "set(2, [|1|], 7); get(2, [|1|])" then failwith "not right value C"

t[2, 1, 0] <- "7" 
let v4 = t[2, 1, 0]
printfn $"v4 = {v4}"
if v4 <> "set(2, [|1; 0|], 7); get(2, [|1; 0|])" then failwith "not right value D"

// Inference defaults to the array in absense of other information 
let f idxs =
    t[ 1, idxs ] <- "7" 

f ([| 2 |] )
let v5 = t[ 1, [| 2 |] ]
printfn $"v5 = {v5}"
if v5 <> "set(1, [|2|], 7); get(1, [|2|])" then failwith "not right value"
    """
     |> ignoreWarnings
     |> compileExeAndRun
     |> shouldSucceed

    [<Fact>]
    let ``Indexer setter via extension can use ParamArray`` () =
        FSharp """
module One
open System

type T() =
    member val v : string = "" with get, set

[<AutoOpen>]
module M =
    type T with 
        member this.Item
            with get ([<ParamArray>] indices: int[]) = 
                $"{this.v}; get(%A{indices})"

            and set ([<ParamArray>] indices: int[]) (value: string) =
                this.v <- $"set(%A{indices}, {value})"

let t = T()

// Using an explicit array is allowed
t[[| 2 |] ] <- "7" 
let v2 = t[[| 2 |]]
printfn $"v2 = {v2}"
if v2 <> "set([|2|], 7); get([|2|])" then failwith "not right value B"


t[1] <- "7" 
let v3 = t[1]
printfn $"v3 = {v3}"
if v3 <> "set([|1|], 7); get([|1|])" then failwith "not right value C"

t[1, 0] <- "7" 
let v4 = t[1, 0]
printfn $"v4 = {v4}"
if v4 <> "set([|1; 0|], 7); get([|1; 0|])" then failwith "not right value D"

// Inference defaults to the array in absense of other information 
let f idxs =
    t[ idxs ] <- "7" 

f ([| 2 |] )
let v5 = t[ [| 2 |] ]
printfn $"v5 = {v5}"
if v5 <> "set([|2|], 7); get([|2|])" then failwith "not right value"


    """
     |> ignoreWarnings
     |> compileExeAndRun
     |> shouldSucceed

#endif