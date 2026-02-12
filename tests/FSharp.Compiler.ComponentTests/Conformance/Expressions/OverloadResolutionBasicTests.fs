// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module OverloadTests

open System
open System.Collections.Generic

type BasicOverload =
    static member Pick(x: int) = "int"
    static member Pick(x: string) = "string"
    static member Pick(x: float) = "float"
    static member Pick(x: bool) = "bool"
    static member Pick<'T>(x: 'T) = "generic<" + typeof<'T>.Name + ">"

type MultiArg =
    static member Pick(a: int, b: int) = "int,int"
    static member Pick(a: string, b: string) = "string,string"
    static member Pick(a: int, b: string) = "int,string"
    static member Pick(a: string, b: int) = "string,int"
    static member Pick<'T>(a: 'T, b: 'T) = "generic<" + typeof<'T>.Name + ">,same"

type ConstrainedCheck =
    static member Pick<'T when 'T :> IComparable>(x: 'T) = "IComparable<" + typeof<'T>.Name + ">"
    static member Pick(x: obj) = "obj"

type OutArgOverload =
    static member TryGet(key: string, [<Runtime.InteropServices.Out>] value: byref<int>) = value <- 42; true
    static member TryGet(key: string, [<Runtime.InteropServices.Out>] value: byref<string>) = value <- "found"; true

type ParamArrayOverload =
    static member Pick([<ParamArray>] args: int[]) = sprintf "int[%d]" args.Length
    static member Pick([<ParamArray>] args: string[]) = sprintf "string[%d]" args.Length
    static member Pick(single: int) = "single-int"
    static member Pick(single: string) = "single-string"

type Animal() = class end
type Dog() = inherit Animal()
type Cat() = inherit Animal()

type HierarchyOverload =
    static member Accept(x: Animal) = "Animal"
    static member Accept(x: Dog) = "Dog"
    static member Accept(x: Cat) = "Cat"
    static member Accept<'T when 'T :> Animal>(items: seq<'T>) = "seq<" + typeof<'T>.Name + ">"

[<AutoOpen>]
module Extensions =
    type String with
        member this.ExtPick(x: int) = "String.ExtPick(int)"
        member this.ExtPick(x: string) = "String.ExtPick(string)"
    type Int32 with
        member this.ExtPick(x: int) = "Int32.ExtPick(int)"
        member this.ExtPick(x: string) = "Int32.ExtPick(string)"

type OptionalOverload =
    static member Pick(x: int, ?y: int) = match y with Some v -> sprintf "int,%d" v | None -> "int,none"
    static member Pick(x: string, ?y: string) = match y with Some v -> sprintf "string,%s" v | None -> "string,none"

type NamedArgOverload =
    static member Pick(first: int, second: string) = "first:int,second:string"
    static member Pick(first: string, second: int) = "first:string,second:int"

type TDCOverload =
    static member Pick(x: int64) = "int64"
    static member Pick(x: int) = "int"
    static member Pick(x: float) = "float"

type TupleOverload =
    static member Pick(x: int * string) = "tuple"
    static member Pick(x: int, y: string) = "separate"

let inline pickRigid<'T> (x: 'T) = BasicOverload.Pick(x)

let cacheStress () =
    let mutable all = true
    for i in 1..100 do if BasicOverload.Pick(i) <> "int" then all <- false
    if all then "all-int" else "MISMATCH"

let cacheAlternating () =
    sprintf "%s,%s,%s,%s,%s,%s" 
        (BasicOverload.Pick 1) (BasicOverload.Pick "a") 
        (BasicOverload.Pick 2) (BasicOverload.Pick "b")
        (BasicOverload.Pick 3) (BasicOverload.Pick "c")

[<EntryPoint>]
let main _ =
    let results = [
        "int",                    BasicOverload.Pick(42)
        "string",                 BasicOverload.Pick("hello")
        "float",                  BasicOverload.Pick(3.14)
        "bool",                   BasicOverload.Pick(true)
        "generic<FSharpList`1>",  BasicOverload.Pick([1;2;3])
        
        "int,int",                MultiArg.Pick(1, 2)
        "string,string",          MultiArg.Pick("a", "b")
        "int,string",             MultiArg.Pick(1, "b")
        "string,int",             MultiArg.Pick("a", 2)
        "generic<Boolean>,same",  MultiArg.Pick(true, false)
        
        "int[0]",                 ParamArrayOverload.Pick([||] : int[])
        "int[3]",                 ParamArrayOverload.Pick(1, 2, 3)
        "string[3]",              ParamArrayOverload.Pick("a", "b", "c")
        "single-int",             ParamArrayOverload.Pick(42)
        "single-string",          ParamArrayOverload.Pick("single")
        
        "Animal",                 HierarchyOverload.Accept(Animal())
        "Dog",                    HierarchyOverload.Accept(Dog())
        "Cat",                    HierarchyOverload.Accept(Cat())
        "seq<Dog>",               HierarchyOverload.Accept([Dog(); Dog()])
        
        "String.ExtPick(int)",    "hello".ExtPick(42)
        "String.ExtPick(string)", "hello".ExtPick("world")
        "Int32.ExtPick(int)",     (5).ExtPick(10)
        "Int32.ExtPick(string)",  (5).ExtPick("ten")
        
        "int,none",               OptionalOverload.Pick(1)
        "int,2",                  OptionalOverload.Pick(1, 2)
        "string,none",            OptionalOverload.Pick("a")
        "string,b",               OptionalOverload.Pick("a", "b")
        
        "all-int",                cacheStress()
        "int,string,int,string,int,string", cacheAlternating()
        
        "generic<Int32>",         pickRigid 42
        "generic<String>",        pickRigid "hello"
        "generic<Boolean>",       pickRigid true
        
        "tuple",                  TupleOverload.Pick((1, "a"))
        "separate",               TupleOverload.Pick(1, "a")
        
        "first:int,second:string",  NamedArgOverload.Pick(1, "a")
        "first:string,second:int",  NamedArgOverload.Pick("a", 1)
        "first:int,second:string",  NamedArgOverload.Pick(first = 1, second = "b")
        
        "obj",                    ConstrainedCheck.Pick(42)
        "obj",                    ConstrainedCheck.Pick("hi")
        
        "int",                    TDCOverload.Pick(42)
        "int64",                  TDCOverload.Pick(42L)
        "float",                  TDCOverload.Pick(3.14)
        
        "success:42",             (let mutable v = 0 in if OutArgOverload.TryGet("k", &v) then sprintf "success:%d" v else "failed")
        "success:found",          (let mutable v = "" in if OutArgOverload.TryGet("k", &v) then sprintf "success:%s" v else "failed")
    ]
    
    let mutable failures = 0
    for (expected, actual) in results do
        if actual = expected then printfn "PASS: %s" actual
        else printfn "FAIL: got %s (expected %s)" actual expected; failures <- failures + 1
    
    if failures = 0 then printfn "All %d tests passed!" results.Length
    else printfn "%d of %d tests failed!" failures results.Length
    failures
