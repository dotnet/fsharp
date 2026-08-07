// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

module RegressionTests =

    [<Fact>]
    let ``No internal errors should be raised``() =
        FSharp """
namespace FSharpBug

type TestItemSeq = 
    static member Test1 item = item
    static member Test2 item = match item with Typo2 x -> x
        """
        |> compile
        |> withErrorCodes [39]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19156
    [<Fact>]
    let ``Generic list comprehension with nested lambda should not cause duplicate entry in type index table``() =
        FSharp """
module Test
open System

let f (start: DateTime) (stop: DateTime) (input: (DateTime * 'a) list) =
    [
        for i in start.Ticks .. stop.Ticks ->
            input |> List.where (fun (k, v) -> true)
    ]
        """
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19156
    [<Fact>]
    let ``Generic array comprehension with nested lambda should not cause duplicate entry in type index table``() =
        FSharp """
module Test
open System

let f (start: DateTime) (stop: DateTime) (input: (DateTime * 'a) list) =
    [|
        for i in start.Ticks .. stop.Ticks ->
            input |> List.where (fun (k, v) -> true)
    |]
        """
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14152
    [<Fact>]
    let ``Issue 14152 - nowarn directive before module declaration should compile`` () =
        FSharp
            """
#nowarn "20"

module XXX.MyModule

let x = 15
            """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/16007
    [<Fact>]
    let ``Issue 16007 - SRTP ctor constraint should not cause value restriction error`` () =
        FSharp """
type T() = class end
let dosmth (a: T) = System.Console.WriteLine(a.ToString())
let inline NEW () = (^a : (new : unit -> ^a) ())
let x = NEW ()
dosmth x
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20203
    // Release-only InvalidProgramException from Seq.collect / yield! over a *struct* seq<'T>,
    // materialised with List.ofSeq / Seq.toList / Seq.toArray. Fixed by boxing the struct
    // sub-collection to seq<'T> before AddMany and using a unit try/finally result type.

    let private structSeqPrelude =
        """
open System
open System.Collections
open System.Collections.Generic

[<Struct>]
type StructSeq(items: int[]) =
    interface IEnumerable<int> with
        member _.GetEnumerator() : IEnumerator<int> = (items :> IEnumerable<int>).GetEnumerator()
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = items.GetEnumerator()
"""

    let private runOptimized body =
        Fsx(structSeqPrelude + "\n" + body)
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    [<Theory>]
    // struct seq<int> via all three collector-lowered materialisers
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.collect id |> List.ofSeq in if r <> [1;2] then failwithf \"%A\" r")>]
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.collect id |> Seq.toList in if r <> [1;2] then failwithf \"%A\" r")>]
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.collect id |> Seq.toArray in if r <> [|1;2|] then failwithf \"%A\" r")>]
    // yield! comprehension forms (regression guards - already worked)
    [<InlineData("let r = [ for a in [ StructSeq [|1;2|] ] do yield! a ] |> List.ofSeq in if r <> [1;2] then failwithf \"%A\" r")>]
    [<InlineData("let r = [| for a in [ StructSeq [|1;2|] ] do yield! a |] in if r <> [|1;2|] then failwithf \"%A\" r")>]
    // non-identity collect mapping
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.collect (fun s -> s) |> List.ofSeq in if r <> [1;2] then failwithf \"%A\" r")>]
    // multiple sub-collections, order preserved
    [<InlineData("let r = [ StructSeq [|1;2|]; StructSeq [|3;4;5|] ] |> Seq.collect id |> List.ofSeq in if r <> [1;2;3;4;5] then failwithf \"%A\" r")>]
    // negatives / no-regression: reference inner collections still work
    [<InlineData("let r = [ [1;2] ] |> Seq.collect id |> List.ofSeq in if r <> [1;2] then failwithf \"%A\" r")>]
    [<InlineData("let r = [ ResizeArray [1;2] ] |> Seq.collect id |> List.ofSeq in if r <> [1;2] then failwithf \"%A\" r")>]
    // Array.ofSeq path (NOT lowered to the collector loop) must still work for a struct seq
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.collect id |> Array.ofSeq in if r <> [|1;2|] then failwithf \"%A\" r")>]
    // Seq.map (no collect) yields a list of one struct
    [<InlineData("let r = [ StructSeq [|1;2|] ] |> Seq.map id |> List.ofSeq in if List.length r <> 1 then failwithf \"%A\" (List.length r)")>]
    // empty cases
    [<InlineData("let r = ([] : StructSeq list) |> Seq.collect id |> List.ofSeq in if r <> [] then failwithf \"%A\" r")>]
    [<InlineData("let r = [ StructSeq [||] ] |> Seq.collect id |> List.ofSeq in if r <> [] then failwithf \"%A\" r")>]
    // boundary: single element and large (no off-by-one in the collector loop)
    [<InlineData("let r = [ StructSeq [|1|] ] |> Seq.collect id |> List.ofSeq in if r <> [1] then failwithf \"%A\" r")>]
    [<InlineData("let r = [ StructSeq [| for i in 1..1000 -> i |] ] |> Seq.collect id |> List.ofSeq in if List.length r <> 1000 || List.head r <> 1 || List.last r <> 1000 then failwithf \"%A\" (List.length r)")>]
    let ``Issue20203 - struct seq materialisation under optimization`` (body: string) =
        runOptimized body

    [<Fact>]
    let ``Issue20203 - generic struct seq element types``() =
        Fsx """
open System.Collections
open System.Collections.Generic

[<Struct>]
type StructSeq<'T>(items: 'T[]) =
    interface IEnumerable<'T> with
        member _.GetEnumerator() : IEnumerator<'T> = (items :> IEnumerable<'T>).GetEnumerator()
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = items.GetEnumerator()

let s = [ StructSeq<string> [|"a";"b"|] ] |> Seq.collect id |> List.ofSeq
if s <> ["a";"b"] then failwithf "strings: %A" s
let n = [ StructSeq<int64> [|1L;2L|] ] |> Seq.collect id |> List.ofSeq
if n <> [1L;2L] then failwithf "int64: %A" n
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Issue20203 - struct seq with disposable enumerator still disposes``() =
        Fsx """
open System.Collections
open System.Collections.Generic

let mutable disposed = 0

type TrackingEnumerator(items: int[]) =
    let inner = (items :> IEnumerable<int>).GetEnumerator()
    interface IEnumerator<int> with
        member _.Current = inner.Current
    interface IEnumerator with
        member _.Current = box inner.Current
        member _.MoveNext() = inner.MoveNext()
        member _.Reset() = inner.Reset()
    interface System.IDisposable with
        member _.Dispose() = disposed <- disposed + 1; inner.Dispose()

[<Struct>]
type StructSeq(items: int[]) =
    interface IEnumerable<int> with
        member _.GetEnumerator() : IEnumerator<int> = new TrackingEnumerator(items) :> IEnumerator<int>
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = (new TrackingEnumerator(items)) :> IEnumerator

let r = [ StructSeq [|1;2|] ] |> Seq.collect id |> List.ofSeq
if r <> [1;2] then failwithf "result: %A" r
if disposed < 1 then failwith "enumerator was not disposed"
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore
