// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
// Adversarial test source for overload resolution cache.
// These tests intentionally try to poison the cache by calling with different types
// that have the same shape but different underlying types.
module CacheBustingTests

open System
open System.Collections.Generic

// Overloads that differ only in generic instantiation
type GenericOverload =
    static member Process<'T>(x: 'T, y: 'T) = typeof<'T>.Name

// Overloads where a cache key computed on one might affect another
type SubtypeOverload =
    static member Accept(x: obj) = "obj"
    static member Accept(x: string) = "string"
    static member Accept(x: int) = "int"
    static member Accept(x: float) = "float"

// Complex generic instantiations
type NestedGeneric =
    static member Process<'T>(x: List<'T>) = "List<" + typeof<'T>.Name + ">"
    static member Process<'T>(x: 'T[]) = "Array<" + typeof<'T>.Name + ">"

// Overloads with ref/out (byref) types
type ByrefOverload =
    static member TryGet(key: string, [<System.Runtime.InteropServices.Out>] result: byref<int>) : bool = 
        result <- 100
        true
    static member TryGet(key: string, [<System.Runtime.InteropServices.Out>] result: byref<string>) : bool = 
        result <- "value"
        true

// Overloads mixing param arrays with normal params  
type MixedParamArray =
    static member Call(x: int, y: int) = "two-int"
    static member Call([<ParamArray>] args: int[]) = sprintf "params-int[%d]" args.Length
    static member Call(x: string, [<ParamArray>] rest: string[]) = sprintf "string+params[%d]" rest.Length

// Tests that intentionally try to confuse the cache

// Test 1: Call same method name with different generic instantiations rapidly
let test_generic_int_int () = GenericOverload.Process(1, 2)
let test_generic_str_str () = GenericOverload.Process("a", "b")
let test_generic_bool_bool () = GenericOverload.Process(true, false)
let test_generic_float_float () = GenericOverload.Process(1.0, 2.0)
// Cycle back to verify cache doesn't corrupt
let test_generic_int_int_2 () = GenericOverload.Process(3, 4)

// Test 2: Subtype overloads - test that specific types are picked over obj
let test_subtype_string () = SubtypeOverload.Accept("hello")
let test_subtype_int () = SubtypeOverload.Accept(42)
let test_subtype_float () = SubtypeOverload.Accept(3.14)
let test_subtype_obj () = SubtypeOverload.Accept(box [1;2;3])  // This MUST pick obj
// Interleave to try to poison cache
let test_subtype_string_2 () = SubtypeOverload.Accept("world")
let test_subtype_int_2 () = SubtypeOverload.Accept(99)

// Test 3: Nested generics with same outer but different inner
let test_nested_list_int () = NestedGeneric.Process([1;2;3] |> List)
let test_nested_list_string () = NestedGeneric.Process(["a";"b"] |> List)
let test_nested_array_int () = NestedGeneric.Process([|1;2;3|])
let test_nested_array_string () = NestedGeneric.Process([|"a";"b"|])

// Test 4: Byref overloads - test that the byref type matters
let test_byref_int () = 
    let mutable result = 0
    if ByrefOverload.TryGet("key", &result) then sprintf "int:%d" result else "failed"
    
let test_byref_string () = 
    let mutable result = ""
    if ByrefOverload.TryGet("key", &result) then sprintf "string:%s" result else "failed"

// Cycle back
let test_byref_int_2 () = 
    let mutable result = 0
    if ByrefOverload.TryGet("other", &result) then sprintf "int:%d" result else "failed"

// Test 5: ParamArray edge cases
let test_mixed_two_int () = MixedParamArray.Call(1, 2)
let test_mixed_three_int () = MixedParamArray.Call(1, 2, 3)  // Should use params
let test_mixed_four_int () = MixedParamArray.Call(1, 2, 3, 4)
let test_mixed_str_params () = MixedParamArray.Call("x", "a", "b", "c")

// Test 6: Stress test - many calls in sequence with varying types
let test_stress_sequence () =
    let results = ResizeArray<string>()
    for i in 1..50 do
        results.Add(SubtypeOverload.Accept(i))
        results.Add(SubtypeOverload.Accept(sprintf "s%d" i))
    // All odd indices should be "string", all even "int"
    let intCorrect = results |> Seq.indexed |> Seq.filter (fun (i,_) -> i % 2 = 0) |> Seq.forall (fun (_,v) -> v = "int")
    let strCorrect = results |> Seq.indexed |> Seq.filter (fun (i,_) -> i % 2 = 1) |> Seq.forall (fun (_,v) -> v = "string")
    if intCorrect && strCorrect then "alternating-correct" else "CORRUPTED"

// Helper to create F# list from seq for NestedGeneric tests
let inline List (xs: 'a seq) = List<'a>(xs)

[<EntryPoint>]
let main _ =
    let results = [
        // Generic instantiations
        "test_generic_int_int", test_generic_int_int(), "Int32"
        "test_generic_str_str", test_generic_str_str(), "String"
        "test_generic_bool_bool", test_generic_bool_bool(), "Boolean"
        "test_generic_float_float", test_generic_float_float(), "Double"
        "test_generic_int_int_2", test_generic_int_int_2(), "Int32"
        
        // Subtype overloads
        "test_subtype_string", test_subtype_string(), "string"
        "test_subtype_int", test_subtype_int(), "int"
        "test_subtype_float", test_subtype_float(), "float"
        "test_subtype_obj", test_subtype_obj(), "obj"
        "test_subtype_string_2", test_subtype_string_2(), "string"
        "test_subtype_int_2", test_subtype_int_2(), "int"
        
        // Nested generics
        "test_nested_list_int", test_nested_list_int(), "List<Int32>"
        "test_nested_list_string", test_nested_list_string(), "List<String>"
        "test_nested_array_int", test_nested_array_int(), "Array<Int32>"
        "test_nested_array_string", test_nested_array_string(), "Array<String>"
        
        // Byref
        "test_byref_int", test_byref_int(), "int:100"
        "test_byref_string", test_byref_string(), "string:value"
        "test_byref_int_2", test_byref_int_2(), "int:100"
        
        // ParamArray
        "test_mixed_two_int", test_mixed_two_int(), "two-int"
        "test_mixed_three_int", test_mixed_three_int(), "params-int[3]"
        "test_mixed_four_int", test_mixed_four_int(), "params-int[4]"
        "test_mixed_str_params", test_mixed_str_params(), "string+params[3]"
        
        // Stress
        "test_stress_sequence", test_stress_sequence(), "alternating-correct"
    ]
    
    let mutable failures = 0
    for (name, actual, expected) in results do
        if actual = expected then
            printfn "PASS: %s = %s" name actual
        else
            printfn "FAIL: %s = %s (expected %s)" name actual expected
            failures <- failures + 1
    
    if failures = 0 then
        printfn "All %d adversarial tests passed!" results.Length
    else
        printfn "%d of %d adversarial tests failed!" failures results.Length
    
    failures
