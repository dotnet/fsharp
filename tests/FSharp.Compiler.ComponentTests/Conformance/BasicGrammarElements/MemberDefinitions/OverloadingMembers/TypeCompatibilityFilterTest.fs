// #Conformance #DeclarationElements #MemberDefinitions #Overloading
// Test that quick type compatibility filtering in overload resolution works correctly
// This tests that overloads with obviously incompatible types are filtered before full type checking

open System
open System.Collections.Generic

// Test class with overloads that have different parameter types
// Quick type compatibility should filter out overloads where:
// - Both types are sealed and different (e.g., int vs string)
// But should NOT filter out overloads where:
// - Either type is generic
// - Callee type is an interface
// - Type-directed conversions might apply

type TypeCompatTest() =
    // Different sealed types - quick filter should distinguish these
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process(x: float) = "float"
    static member Process(x: bool) = "bool"
    static member Process(x: byte) = "byte"
    
    // Generic overload - should never be filtered out
    static member Generic<'T>(x: 'T) = sprintf "generic-%s" (typeof<'T>.Name)
    
    // Interface parameter - should not be filtered (caller might implement it)
    static member WithInterface(x: IComparable) = "IComparable"
    static member WithInterface(x: IEnumerable<int>) = "IEnumerable<int>"
    
    // Object parameter - should not be filtered (anything can be object)
    static member WithObject(x: obj) = "obj"
    
    // Tuple parameters - different lengths should be incompatible
    static member WithTuple(x: int * int) = "tuple2"
    static member WithTuple(x: int * int * int) = "tuple3"
    
    // Array parameters - different ranks should be incompatible
    static member WithArray(x: int[]) = "array1d"
    static member WithArray(x: int[,]) = "array2d"
    
    // Multiple parameter overloads with different types
    static member Multi(x: int, y: int) = "int-int"
    static member Multi(x: string, y: string) = "string-string"
    static member Multi(x: int, y: string) = "int-string"
    static member Multi(x: string, y: int) = "string-int"
    
    // Nullable - should allow T -> Nullable<T> conversion
    static member WithNullable(x: Nullable<int>) = "nullable-int"
    static member WithNullable(x: Nullable<float>) = "nullable-float"
    
    // Type-directed conversions: int -> int64, int -> float, int -> nativeint
    static member NumericConversions(x: int64) = "int64"
    static member NumericConversions(x: nativeint) = "nativeint"
    // static member NumericConversions(x: float) = "float-conv"  // Commented to avoid ambiguity

// Tests for sealed type filtering
if TypeCompatTest.Process(42) <> "int" then failwith "Failed: Process int"
if TypeCompatTest.Process("hello") <> "string" then failwith "Failed: Process string"
if TypeCompatTest.Process(3.14) <> "float" then failwith "Failed: Process float"
if TypeCompatTest.Process(true) <> "bool" then failwith "Failed: Process bool"
if TypeCompatTest.Process(42uy) <> "byte" then failwith "Failed: Process byte"

// Tests for generic overload
if TypeCompatTest.Generic(42) <> "generic-Int32" then failwith "Failed: Generic int"
if TypeCompatTest.Generic("test") <> "generic-String" then failwith "Failed: Generic string"

// Tests for interface parameters - int implements IComparable
if TypeCompatTest.WithInterface(42 :> IComparable) <> "IComparable" then failwith "Failed: WithInterface IComparable"
if TypeCompatTest.WithInterface([1; 2; 3] :> IEnumerable<int>) <> "IEnumerable<int>" then failwith "Failed: WithInterface IEnumerable"

// Tests for object parameter
if TypeCompatTest.WithObject(42) <> "obj" then failwith "Failed: WithObject int"
if TypeCompatTest.WithObject("test") <> "obj" then failwith "Failed: WithObject string"

// Tests for tuple parameters
if TypeCompatTest.WithTuple((1, 2)) <> "tuple2" then failwith "Failed: WithTuple 2"
if TypeCompatTest.WithTuple((1, 2, 3)) <> "tuple3" then failwith "Failed: WithTuple 3"

// Tests for array parameters
if TypeCompatTest.WithArray([| 1; 2; 3 |]) <> "array1d" then failwith "Failed: WithArray 1d"
if TypeCompatTest.WithArray(Array2D.init 2 2 (fun i j -> i + j)) <> "array2d" then failwith "Failed: WithArray 2d"

// Tests for multi-parameter overloads
if TypeCompatTest.Multi(1, 2) <> "int-int" then failwith "Failed: Multi int-int"
if TypeCompatTest.Multi("a", "b") <> "string-string" then failwith "Failed: Multi string-string"
if TypeCompatTest.Multi(1, "b") <> "int-string" then failwith "Failed: Multi int-string"
if TypeCompatTest.Multi("a", 2) <> "string-int" then failwith "Failed: Multi string-int"

// Tests for nullable
if TypeCompatTest.WithNullable(Nullable<int>(42)) <> "nullable-int" then failwith "Failed: WithNullable int"
if TypeCompatTest.WithNullable(Nullable<float>(3.14)) <> "nullable-float" then failwith "Failed: WithNullable float"

// Tests for numeric conversions (int -> int64, int -> nativeint)
// Note: These require type annotations because multiple overloads might match
if TypeCompatTest.NumericConversions(42L) <> "int64" then failwith "Failed: NumericConversions int64"
if TypeCompatTest.NumericConversions(42n) <> "nativeint" then failwith "Failed: NumericConversions nativeint"

printfn "All type compatibility filtering tests passed!"
