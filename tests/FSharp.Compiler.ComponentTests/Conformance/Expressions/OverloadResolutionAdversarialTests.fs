// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module CacheBustingTests

open System
open System.Collections.Generic

type GenericOverload =
    static member Process<'T>(x: 'T, y: 'T) = typeof<'T>.Name

type SubtypeOverload =
    static member Accept(x: obj) = "obj"
    static member Accept(x: string) = "string"
    static member Accept(x: int) = "int"
    static member Accept(x: float) = "float"

type NestedGeneric =
    static member Process<'T>(x: List<'T>) = "List<" + typeof<'T>.Name + ">"
    static member Process<'T>(x: 'T[]) = "Array<" + typeof<'T>.Name + ">"

type ByrefOverload =
    static member TryGet(key: string, [<Runtime.InteropServices.Out>] result: byref<int>) = result <- 100; true
    static member TryGet(key: string, [<Runtime.InteropServices.Out>] result: byref<string>) = result <- "value"; true

type MixedParamArray =
    static member Call(x: int, y: int) = "two-int"
    static member Call([<ParamArray>] args: int[]) = sprintf "params-int[%d]" args.Length
    static member Call(x: string, [<ParamArray>] rest: string[]) = sprintf "string+params[%d]" rest.Length

let inline List (xs: 'a seq) = List<'a>(xs)

let stressSequence () =
    let results = ResizeArray<string>()
    for i in 1..50 do
        results.Add(SubtypeOverload.Accept(i))
        results.Add(SubtypeOverload.Accept(sprintf "s%d" i))
    let intOk = results |> Seq.indexed |> Seq.filter (fun (i,_) -> i % 2 = 0) |> Seq.forall (fun (_,v) -> v = "int")
    let strOk = results |> Seq.indexed |> Seq.filter (fun (i,_) -> i % 2 = 1) |> Seq.forall (fun (_,v) -> v = "string")
    if intOk && strOk then "alternating-correct" else "CORRUPTED"

[<EntryPoint>]
let main _ =
    let results = [
        "Int32",    GenericOverload.Process(1, 2)
        "String",   GenericOverload.Process("a", "b")
        "Boolean",  GenericOverload.Process(true, false)
        "Double",   GenericOverload.Process(1.0, 2.0)
        "Int32",    GenericOverload.Process(3, 4)
        
        "string",   SubtypeOverload.Accept("hello")
        "int",      SubtypeOverload.Accept(42)
        "float",    SubtypeOverload.Accept(3.14)
        "obj",      SubtypeOverload.Accept(box [1;2;3])
        "string",   SubtypeOverload.Accept("world")
        "int",      SubtypeOverload.Accept(99)
        
        "List<Int32>",    NestedGeneric.Process([1;2;3] |> List)
        "List<String>",   NestedGeneric.Process(["a";"b"] |> List)
        "Array<Int32>",   NestedGeneric.Process([|1;2;3|])
        "Array<String>",  NestedGeneric.Process([|"a";"b"|])
        
        "100",      (let mutable v = 0 in if ByrefOverload.TryGet("k", &v) then sprintf "%d" v else "failed")
        "value",    (let mutable v = "" in if ByrefOverload.TryGet("k", &v) then v else "failed")
        "100",      (let mutable v = 0 in if ByrefOverload.TryGet("x", &v) then sprintf "%d" v else "failed")
        
        "two-int",        MixedParamArray.Call(1, 2)
        "params-int[3]",  MixedParamArray.Call(1, 2, 3)
        "params-int[4]",  MixedParamArray.Call(1, 2, 3, 4)
        "string+params[3]", MixedParamArray.Call("x", "a", "b", "c")
        
        "alternating-correct", stressSequence()
    ]
    
    let mutable failures = 0
    for (expected, actual) in results do
        if actual = expected then printfn "PASS: %s" actual
        else printfn "FAIL: got %s (expected %s)" actual expected; failures <- failures + 1
    
    if failures = 0 then printfn "All %d adversarial tests passed!" results.Length
    else printfn "%d of %d adversarial tests failed!" failures results.Length
    failures
