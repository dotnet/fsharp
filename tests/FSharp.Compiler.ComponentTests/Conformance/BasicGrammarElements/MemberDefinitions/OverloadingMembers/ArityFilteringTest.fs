// #Conformance #DeclarationElements #MemberDefinitions #Overloading

open System

type OverloadTest() =
    member this.Method() = "no-args"
    member this.Method(x: int) = "one-int"
    member this.Method(x: int, y: int) = "two-ints"
    member this.Method(x: int, y: int, z: int) = "three-ints"
    
    static member StaticMethod() = "static-no-args"
    static member StaticMethod(x: int) = "static-one-int"
    static member StaticMethod(x: int, y: int) = "static-two-ints"
    
    member this.OptMethod(x: int, ?y: int) = 
        match y with
        | Some v -> sprintf "opt-%d-%d" x v
        | None -> sprintf "opt-%d-none" x
    
    member this.ParamArrayMethod([<ParamArray>] args: int[]) = 
        sprintf "params-%d" args.Length

// Simulate Assert.Equal-like pattern with many overloads at different arities
type MockAssert =
    static member Equal(expected: int, actual: int) = "int-int"
    static member Equal(expected: string, actual: string) = "string-string"
    static member Equal(expected: float, actual: float) = "float-float"
    static member Equal(expected: obj, actual: obj) = "obj-obj"
    
    static member Equal(expected: float, actual: float, precision: int) = "float-float-precision"
    static member Equal(expected: int, actual: int, comparer: System.Collections.Generic.IEqualityComparer<int>) = "int-int-comparer"
    static member Equal(expected: string, actual: string, comparer: System.Collections.Generic.IEqualityComparer<string>) = "string-string-comparer"
    
    static member Single(x: int) = "single-int"
    
    static member Quad(a: int, b: int, c: int, d: int) = "quad"
    
    static member WithCallerInfo(x: int, [<System.Runtime.CompilerServices.CallerMemberName>] ?callerName: string) =
        match callerName with
        | Some n -> sprintf "caller-%s" n
        | None -> "caller-none"

let test = OverloadTest()

if test.Method() <> "no-args" then failwith "Failed: no-args"
if test.Method(1) <> "one-int" then failwith "Failed: one-int"
if test.Method(1, 2) <> "two-ints" then failwith "Failed: two-ints"
if test.Method(1, 2, 3) <> "three-ints" then failwith "Failed: three-ints"

if OverloadTest.StaticMethod() <> "static-no-args" then failwith "Failed: static-no-args"
if OverloadTest.StaticMethod(1) <> "static-one-int" then failwith "Failed: static-one-int"
if OverloadTest.StaticMethod(1, 2) <> "static-two-ints" then failwith "Failed: static-two-ints"

if test.OptMethod(42) <> "opt-42-none" then failwith "Failed: opt with none"
if test.OptMethod(42, 10) <> "opt-42-10" then failwith "Failed: opt with value"

if test.ParamArrayMethod() <> "params-0" then failwith "Failed: params-0"
if test.ParamArrayMethod(1) <> "params-1" then failwith "Failed: params-1"
if test.ParamArrayMethod(1, 2) <> "params-2" then failwith "Failed: params-2"
if test.ParamArrayMethod(1, 2, 3, 4, 5) <> "params-5" then failwith "Failed: params-5"

if MockAssert.Equal(1, 2) <> "int-int" then failwith "Failed: Equal int-int"
if MockAssert.Equal("a", "b") <> "string-string" then failwith "Failed: Equal string-string"
if MockAssert.Equal(1.0, 2.0) <> "float-float" then failwith "Failed: Equal float-float"

if MockAssert.Equal(1.0, 2.0, 5) <> "float-float-precision" then failwith "Failed: Equal with precision"

if MockAssert.WithCallerInfo(42).StartsWith("caller-") |> not then failwith "Failed: WithCallerInfo"

printfn "All arity filtering tests passed!"
