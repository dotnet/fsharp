// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
// Test source for overload resolution cache correctness.
// Each overload returns a unique string identifying which overload was picked.
module OverloadTests

open System
open System.Collections.Generic

// ========================================
// Test Type Definitions
// ========================================

// Basic overloads with different primitive types
type BasicOverload =
    static member Pick(x: int) = "int"
    static member Pick(x: string) = "string"
    static member Pick(x: float) = "float"
    static member Pick(x: bool) = "bool"
    static member Pick<'T>(x: 'T) = "generic<" + typeof<'T>.Name + ">"

// Overloads with multiple arguments
type MultiArg =
    static member Pick(a: int, b: int) = "int,int"
    static member Pick(a: string, b: string) = "string,string"
    static member Pick(a: int, b: string) = "int,string"
    static member Pick(a: string, b: int) = "string,int"
    static member Pick<'T>(a: 'T, b: 'T) = "generic<" + typeof<'T>.Name + ">,same"

// Overloads with constraints - constraints affect which is picked at call site
// Note: In F# we can't have two generic methods differing only by constraint,
// so we test constrained vs non-constrained differently
type ConstrainedCheck =
    static member Pick<'T when 'T :> IComparable>(x: 'T) = "IComparable<" + typeof<'T>.Name + ">"
    static member Pick(x: obj) = "obj"

// Overloads with out arguments (affects return type)
type OutArgOverload =
    static member TryGet(key: string, [<System.Runtime.InteropServices.Out>] value: byref<int>) = 
        value <- 42
        true
    static member TryGet(key: string, [<System.Runtime.InteropServices.Out>] value: byref<string>) = 
        value <- "found"
        true

// ParamArray overloads
type ParamArrayOverload =
    static member Pick([<ParamArray>] args: int[]) = sprintf "int[%d]" args.Length
    static member Pick([<ParamArray>] args: string[]) = sprintf "string[%d]" args.Length
    static member Pick(single: int) = "single-int"
    static member Pick(single: string) = "single-string"

// Type hierarchy for subsumption tests
type Animal() = class end
type Dog() = inherit Animal()
type Cat() = inherit Animal()

type HierarchyOverload =
    static member Accept(x: Animal) = "Animal"
    static member Accept(x: Dog) = "Dog"
    static member Accept(x: Cat) = "Cat"
    static member Accept<'T when 'T :> Animal>(items: seq<'T>) = "seq<" + typeof<'T>.Name + ">"

// Extension methods
[<AutoOpen>]
module Extensions =
    type String with
        member this.ExtPick(x: int) = "String.ExtPick(int)"
        member this.ExtPick(x: string) = "String.ExtPick(string)"
    
    type Int32 with
        member this.ExtPick(x: int) = "Int32.ExtPick(int)"
        member this.ExtPick(x: string) = "Int32.ExtPick(string)"

// Optional arguments
type OptionalOverload =
    static member Pick(x: int, ?y: int) = 
        match y with Some v -> sprintf "int,%d" v | None -> "int,none"
    static member Pick(x: string, ?y: string) = 
        match y with Some v -> sprintf "string,%s" v | None -> "string,none"

// Named arguments - test with different types
type NamedArgOverload =
    static member Pick(first: int, second: string) = "first:int,second:string"
    static member Pick(first: string, second: int) = "first:string,second:int"

// Type-directed conversions
type TDCOverload =
    static member Pick(x: int64) = "int64"
    static member Pick(x: int) = "int"
    static member Pick(x: float) = "float"

// Tuple vs individual args
type TupleOverload =
    static member Pick(x: int * string) = "tuple"
    static member Pick(x: int, y: string) = "separate"

// ========================================
// Test Functions - Each returns expected result string
// ========================================

// Test 1: Basic type-specific overloads
let test_basic_int () = BasicOverload.Pick(42)
let test_basic_string () = BasicOverload.Pick("hello")
let test_basic_float () = BasicOverload.Pick(3.14)
let test_basic_bool () = BasicOverload.Pick(true)
let test_basic_generic () = BasicOverload.Pick([1;2;3])  // No specific overload for list

// Test 2: Multi-argument overloads
let test_multi_int_int () = MultiArg.Pick(1, 2)
let test_multi_string_string () = MultiArg.Pick("a", "b")
let test_multi_int_string () = MultiArg.Pick(1, "b")
let test_multi_string_int () = MultiArg.Pick("a", 2)
let test_multi_generic_bool () = MultiArg.Pick(true, false)

// Test 3: ParamArray overloads
let test_param_empty_int () = ParamArrayOverload.Pick([||] : int[])
let test_param_many_int () = ParamArrayOverload.Pick(1, 2, 3)
let test_param_many_string () = ParamArrayOverload.Pick("a", "b", "c")
let test_param_single_int () = ParamArrayOverload.Pick(42)
let test_param_single_string () = ParamArrayOverload.Pick("single")

// Test 4: Type hierarchy / subsumption
let test_hierarchy_animal () = HierarchyOverload.Accept(Animal())
let test_hierarchy_dog () = HierarchyOverload.Accept(Dog())
let test_hierarchy_cat () = HierarchyOverload.Accept(Cat())
let test_hierarchy_seq_dog () = HierarchyOverload.Accept([Dog(); Dog()])

// Test 5: Extension methods
let test_ext_string_int () = "hello".ExtPick(42)
let test_ext_string_string () = "hello".ExtPick("world")
let test_ext_int_int () = (5).ExtPick(10)
let test_ext_int_string () = (5).ExtPick("ten")

// Test 6: Optional arguments
let test_opt_int_none () = OptionalOverload.Pick(1)
let test_opt_int_some () = OptionalOverload.Pick(1, 2)
let test_opt_string_none () = OptionalOverload.Pick("a")
let test_opt_string_some () = OptionalOverload.Pick("a", "b")

// Test 7: Caching stress - same overload many times
let test_cache_stress () =
    let mutable results = []
    for i in 1..100 do
        results <- BasicOverload.Pick(i) :: results
    // All should be "int"
    if results |> List.forall ((=) "int") then "all-int" else "MISMATCH"

// Test 8: Alternating types - cache should not cross-contaminate
let test_cache_alternating () =
    let r1 = BasicOverload.Pick(1)
    let r2 = BasicOverload.Pick("a")
    let r3 = BasicOverload.Pick(2)
    let r4 = BasicOverload.Pick("b")
    let r5 = BasicOverload.Pick(3)
    let r6 = BasicOverload.Pick("c")
    sprintf "%s,%s,%s,%s,%s,%s" r1 r2 r3 r4 r5 r6

// Test 9: Generic function with rigid type parameter
let inline pickRigid<'T> (x: 'T) = BasicOverload.Pick(x)
let test_rigid_int () = pickRigid 42
let test_rigid_string () = pickRigid "hello"
let test_rigid_bool () = pickRigid true

// Test 10: Tuple overloads
let test_tuple_as_tuple () = TupleOverload.Pick((1, "a"))
let test_tuple_as_args () = TupleOverload.Pick(1, "a")

// Test 11: Named arguments
let test_named_positional_1 () = NamedArgOverload.Pick(1, "a")
let test_named_positional_2 () = NamedArgOverload.Pick("a", 1)
let test_named_explicit () = NamedArgOverload.Pick(first = 1, second = "b")

// Test 12: Constrained vs unconstrained
let test_constrained_int () = ConstrainedCheck.Pick(42)  // int :> IComparable
let test_constrained_string () = ConstrainedCheck.Pick("hi")  // string :> IComparable

// Test 13: Type-directed conversions
let test_tdc_int () = TDCOverload.Pick(42)  // should pick int
let test_tdc_int64 () = TDCOverload.Pick(42L)  // should pick int64
let test_tdc_float () = TDCOverload.Pick(3.14)  // should pick float

// ========================================
// Main - Run all tests and collect results
// ========================================

[<EntryPoint>]
let main _ =
    let results = [
        // Basic overloads
        "test_basic_int", test_basic_int(), "int"
        "test_basic_string", test_basic_string(), "string"
        "test_basic_float", test_basic_float(), "float"
        "test_basic_bool", test_basic_bool(), "bool"
        "test_basic_generic", test_basic_generic(), "generic<FSharpList`1>"
        
        // Multi-arg overloads
        "test_multi_int_int", test_multi_int_int(), "int,int"
        "test_multi_string_string", test_multi_string_string(), "string,string"
        "test_multi_int_string", test_multi_int_string(), "int,string"
        "test_multi_string_int", test_multi_string_int(), "string,int"
        "test_multi_generic_bool", test_multi_generic_bool(), "generic<Boolean>,same"
        
        // ParamArray
        "test_param_empty_int", test_param_empty_int(), "int[0]"
        "test_param_many_int", test_param_many_int(), "int[3]"
        "test_param_many_string", test_param_many_string(), "string[3]"
        "test_param_single_int", test_param_single_int(), "single-int"
        "test_param_single_string", test_param_single_string(), "single-string"
        
        // Hierarchy
        "test_hierarchy_animal", test_hierarchy_animal(), "Animal"
        "test_hierarchy_dog", test_hierarchy_dog(), "Dog"
        "test_hierarchy_cat", test_hierarchy_cat(), "Cat"
        "test_hierarchy_seq_dog", test_hierarchy_seq_dog(), "seq<Dog>"
        
        // Extension methods
        "test_ext_string_int", test_ext_string_int(), "String.ExtPick(int)"
        "test_ext_string_string", test_ext_string_string(), "String.ExtPick(string)"
        "test_ext_int_int", test_ext_int_int(), "Int32.ExtPick(int)"
        "test_ext_int_string", test_ext_int_string(), "Int32.ExtPick(string)"
        
        // Optional args
        "test_opt_int_none", test_opt_int_none(), "int,none"
        "test_opt_int_some", test_opt_int_some(), "int,2"
        "test_opt_string_none", test_opt_string_none(), "string,none"
        "test_opt_string_some", test_opt_string_some(), "string,b"
        
        // Cache stress
        "test_cache_stress", test_cache_stress(), "all-int"
        
        // Alternating types
        "test_cache_alternating", test_cache_alternating(), "int,string,int,string,int,string"
        
        // Rigid typars - inline generics get the generic overload, not specific
        "test_rigid_int", test_rigid_int(), "generic<Int32>"
        "test_rigid_string", test_rigid_string(), "generic<String>"
        "test_rigid_bool", test_rigid_bool(), "generic<Boolean>"
        
        // Tuples
        "test_tuple_as_tuple", test_tuple_as_tuple(), "tuple"
        "test_tuple_as_args", test_tuple_as_args(), "separate"
        
        // Named arguments
        "test_named_positional_1", test_named_positional_1(), "first:int,second:string"
        "test_named_positional_2", test_named_positional_2(), "first:string,second:int"
        "test_named_explicit", test_named_explicit(), "first:int,second:string"
        
        // Constrained check - obj overload is simpler, compiler picks it
        "test_constrained_int", test_constrained_int(), "obj"
        "test_constrained_string", test_constrained_string(), "obj"
        
        // Type-directed conversions
        "test_tdc_int", test_tdc_int(), "int"
        "test_tdc_int64", test_tdc_int64(), "int64"
        "test_tdc_float", test_tdc_float(), "float"
    ]
    
    let mutable failures = 0
    for (name, actual, expected) in results do
        if actual = expected then
            printfn "PASS: %s = %s" name actual
        else
            printfn "FAIL: %s = %s (expected %s)" name actual expected
            failures <- failures + 1
    
    if failures = 0 then
        printfn "All %d tests passed!" results.Length
    else
        printfn "%d of %d tests failed!" failures results.Length
    
    failures
