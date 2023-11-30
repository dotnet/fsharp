// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:5172
// Title: "method" attribute target is not recognized
// Descr: Verify that attribute target 'method' is recognized by F# compiler correctly.
open System

[<AttributeUsage(AttributeTargets.Method)>]
type MethodOnlyAttribute() = 
  inherit Attribute()

[<MethodOnly>]
let someValue = "someValue" // Should fail

[<MethodOnly>]
let i, j, k = (1, 2, 3) // Should fail

[<MethodOnly>]
let someFunction () = "someFunction"

[<MethodOnly>]
let someFunction2 a = a + 1

[<MethodOnly>]
let someFunction3 (a, b) = a + b

[<MethodOnly>]
let someFunction4 (a: int) : int = a + 1

[<MethodOnly>]
let makeList a b = [ a; b ]

[<MethodOnly>]
let someTypedFunction<'a> = "someTypedFunction"

[<MethodOnly>]
let someTypedFunction2<'a> (x : 'a) = "someTypedFunction2"

[<MethodOnly>]
let someTypedFunction3 = fun x -> x

[<MethodOnly>]
let someTypedFunction4 = id

[<MethodOnly>]
let __someTypedFunction5<'a> = false

[<MethodOnly>]
let __someTypedFunction6<'a> : bool  = false

[<MethodOnly>]
let ``someValue2`` = "someValue" // Should fail