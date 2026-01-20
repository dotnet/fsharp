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

printfn "All arity filtering tests passed!"
