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
    // Release-only System.InvalidProgramException from Seq.collect / yield! over a *struct*
    // collection materialised with List.ofSeq / Seq.toList / Seq.toArray.
    // Hermetic struct seq so the tests don't depend on System.Collections.Immutable.
    let private structSeqPrelude = """
open System.Collections
open System.Collections.Generic

[<Struct>]
type StructSeq(items: int[]) =
    member _.Items = items
    interface IEnumerable<int> with
        member _.GetEnumerator() : IEnumerator<int> = (items :> IEnumerable<int>).GetEnumerator()
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = items.GetEnumerator()

[<Struct>]
type StructSeqG<'T>(items: 'T[]) =
    interface IEnumerable<'T> with
        member _.GetEnumerator() : IEnumerator<'T> = (items :> IEnumerable<'T>).GetEnumerator()
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = items.GetEnumerator()
"""

    let private runStruct (body: string) =
        FSharp (structSeqPrelude + body)
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    // Primary: Seq.collect id over a struct seq, materialised three ways.
    [<Theory>]
    [<InlineData("List.ofSeq")>]
    [<InlineData("Seq.toList")>]
    [<InlineData("Seq.toArray")>]
    let ``20203 Seq.collect id over struct seq materialised`` (materializer: string) =
        runStruct $"""
let xs = [ StructSeq [|1;2|] ]
let result = xs |> Seq.collect id |> {materializer}
if Seq.length result <> 2 then failwithf "expected length 2, got %%d" (Seq.length result)
if (result |> Seq.toList) <> [1;2] then failwithf "wrong contents: %%A" (result |> Seq.toList)
"""

    [<Fact>]
    let ``20203 yield! comprehension over struct seq (list) still works`` () =
        runStruct """
let xs = [ StructSeq [|1;2|] ]
let result = [ for a in xs do yield! a ] |> List.ofSeq
if result <> [1;2] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 yield! comprehension over struct seq (array) still works`` () =
        runStruct """
let xs = [ StructSeq [|1;2|] ]
let result = [| for a in xs do yield! a |]
if List.ofArray result <> [1;2] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 Seq.collect non-identity mapping over struct seq`` () =
        runStruct """
let xs = [ StructSeq [|1;2|] ]
let result = xs |> Seq.collect (fun s -> s) |> List.ofSeq
if result <> [1;2] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 multiple struct sub-collections preserve order`` () =
        runStruct """
let xs = [ StructSeq [|1;2|]; StructSeq [|3;4;5|] ]
let result = xs |> Seq.collect id |> List.ofSeq
if result <> [1;2;3;4;5] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 generic struct seq of reference element`` () =
        runStruct """
let xs = [ StructSeqG<string> [|"a";"b"|] ]
let result = xs |> Seq.collect id |> List.ofSeq
if result <> ["a";"b"] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 generic struct seq of struct element (int64)`` () =
        runStruct """
let xs = [ StructSeqG<int64> [|1L;2L|] ]
let result = xs |> Seq.collect id |> List.ofSeq
if result <> [1L;2L] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 disposable enumerator path runs Dispose`` () =
        runStruct """
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
        member _.Dispose() =
            disposed <- disposed + 1
            inner.Dispose()

[<Struct>]
type DisposableStructSeq(items: int[]) =
    interface IEnumerable<int> with
        member _.GetEnumerator() : IEnumerator<int> = new TrackingEnumerator(items) :> IEnumerator<int>
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = new TrackingEnumerator(items) :> IEnumerator

let xs = [ DisposableStructSeq [|1;2;3|] ]
let result = xs |> Seq.collect id |> List.ofSeq
if result <> [1;2;3] then failwithf "wrong contents: %A" result
if disposed < 1 then failwith "Dispose was not called"
"""

    // Negatives / no-regression: reference inner collections must keep working.
    [<Fact>]
    let ``20203 reference inner list still works`` () =
        FSharp """
module Test
let result = [ [1;2] ] |> Seq.collect id |> List.ofSeq
if result <> [1;2] then failwithf "wrong contents: %A" result
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``20203 reference inner ResizeArray still works`` () =
        FSharp """
module Test
let result = [ System.Collections.Generic.List<int>([1;2]) ] |> Seq.collect id |> List.ofSeq
if result <> [1;2] then failwithf "wrong contents: %A" result
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``20203 Array.ofSeq path over struct seq still works`` () =
        runStruct """
let xs = [ StructSeq [|1;2|] ]
let result = xs |> Seq.collect id |> Array.ofSeq
if List.ofArray result <> [1;2] then failwithf "wrong contents: %A" result
"""

    [<Fact>]
    let ``20203 Seq.map (no collect) over struct seq still works`` () =
        runStruct """
let xs = [ StructSeq [|1;2|] ]
let result = xs |> Seq.map id |> List.ofSeq
if List.length result <> 1 then failwithf "expected 1 struct, got %d" (List.length result)
"""

    [<Fact>]
    let ``20203 empty outer and empty struct sub-collection`` () =
        runStruct """
let empty1 : StructSeq list = []
if (empty1 |> Seq.collect id |> List.ofSeq) <> [] then failwith "empty outer failed"
let xs = [ StructSeq [||] ]
if (xs |> Seq.collect id |> List.ofSeq) <> [] then failwith "empty sub-collection failed"
"""

    [<Fact>]
    let ``20203 boundary single and large struct sub-collections`` () =
        runStruct """
let single = [ StructSeq [|42|] ] |> Seq.collect id |> List.ofSeq
if single <> [42] then failwithf "single failed: %A" single
let big = [| 1 .. 1000 |]
let large = [ StructSeq big ] |> Seq.collect id |> List.ofSeq
if List.length large <> 1000 then failwithf "expected 1000, got %d" (List.length large)
if large.Head <> 1 || (List.last large) <> 1000 then failwith "large order wrong"
"""
