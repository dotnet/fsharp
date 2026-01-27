// #Conformance #DeclarationElements #MemberDefinitions #Overloading
// Test that early arity filtering in overload resolution works correctly
// This tests various edge cases around argument count matching

open System

// Test class with various overloads including different arities, optional params, and param arrays
type OverloadTest() =
    // Different arities
    member this.Method() = "no-args"
    member this.Method(x: int) = "one-int"
    member this.Method(x: int, y: int) = "two-ints"
    member this.Method(x: int, y: int, z: int) = "three-ints"
    
    // Static variants
    static member StaticMethod() = "static-no-args"
    static member StaticMethod(x: int) = "static-one-int"
    static member StaticMethod(x: int, y: int) = "static-two-ints"
    
    // Optional parameters
    member this.OptMethod(x: int, ?y: int) = 
        match y with
        | Some v -> sprintf "opt-%d-%d" x v
        | None -> sprintf "opt-%d-none" x
    
    // Param array
    member this.ParamArrayMethod([<ParamArray>] args: int[]) = 
        sprintf "params-%d" args.Length

// Simulate Assert.Equal-like pattern with many overloads at different arities
// This is the pattern where early arity filtering provides the most benefit
type MockAssert =
    // 2-arg overloads (most common case)
    static member Equal(expected: int, actual: int) = "int-int"
    static member Equal(expected: string, actual: string) = "string-string"
    static member Equal(expected: float, actual: float) = "float-float"
    static member Equal(expected: obj, actual: obj) = "obj-obj"
    
    // 3-arg overloads (with comparer or precision)
    static member Equal(expected: float, actual: float, precision: int) = "float-float-precision"
    static member Equal(expected: int, actual: int, comparer: System.Collections.Generic.IEqualityComparer<int>) = "int-int-comparer"
    static member Equal(expected: string, actual: string, comparer: System.Collections.Generic.IEqualityComparer<string>) = "string-string-comparer"
    
    // 1-arg overload (edge case - filtered out when caller provides 2 args)
    static member Single(x: int) = "single-int"
    
    // 4-arg overload (filtered out when caller provides 2 args)
    static member Quad(a: int, b: int, c: int, d: int) = "quad"
    
    // CallerInfo parameter (should not count as required)
    static member WithCallerInfo(x: int, [<System.Runtime.CompilerServices.CallerMemberName>] ?callerName: string) =
        match callerName with
        | Some n -> sprintf "caller-%s" n
        | None -> "caller-none"

// Test instance methods with different arities
let test = OverloadTest()

if test.Method() <> "no-args" then failwith "Failed: no-args"
if test.Method(1) <> "one-int" then failwith "Failed: one-int"
if test.Method(1, 2) <> "two-ints" then failwith "Failed: two-ints"
if test.Method(1, 2, 3) <> "three-ints" then failwith "Failed: three-ints"

// Test static methods
if OverloadTest.StaticMethod() <> "static-no-args" then failwith "Failed: static-no-args"
if OverloadTest.StaticMethod(1) <> "static-one-int" then failwith "Failed: static-one-int"
if OverloadTest.StaticMethod(1, 2) <> "static-two-ints" then failwith "Failed: static-two-ints"

// Test optional parameters - caller provides fewer args
if test.OptMethod(42) <> "opt-42-none" then failwith "Failed: opt with none"
if test.OptMethod(42, 10) <> "opt-42-10" then failwith "Failed: opt with value"

// Test param array - caller can provide more args
if test.ParamArrayMethod() <> "params-0" then failwith "Failed: params-0"
if test.ParamArrayMethod(1) <> "params-1" then failwith "Failed: params-1"
if test.ParamArrayMethod(1, 2) <> "params-2" then failwith "Failed: params-2"
if test.ParamArrayMethod(1, 2, 3, 4, 5) <> "params-5" then failwith "Failed: params-5"

// Test Assert.Equal-like pattern - the arity filter should eliminate:
// - Single (1 arg) when we provide 2 args
// - Quad (4 args) when we provide 2 args
// - 3-arg overloads when we provide 2 args
// This leaves only the 2-arg overloads for type checking
if MockAssert.Equal(1, 2) <> "int-int" then failwith "Failed: Equal int-int"
if MockAssert.Equal("a", "b") <> "string-string" then failwith "Failed: Equal string-string"
if MockAssert.Equal(1.0, 2.0) <> "float-float" then failwith "Failed: Equal float-float"

// Test 3-arg overloads work correctly
if MockAssert.Equal(1.0, 2.0, 5) <> "float-float-precision" then failwith "Failed: Equal with precision"

// Test CallerInfo parameter - should work with just 1 explicit arg
if MockAssert.WithCallerInfo(42).StartsWith("caller-") |> not then failwith "Failed: WithCallerInfo"

printfn "All arity filtering tests passed!"
