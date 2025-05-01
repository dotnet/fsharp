// This tests that AttributeTargets.Field is not allowed in let function bindings

open System

[<AttributeUsage(AttributeTargets.Field)>]
type FieldOnlyAttribute() = 
   inherit Attribute()

[<FieldOnly>]  // Should fail
let func1 () = "someFunction"

[<FieldOnly>] // Should fail
let func2 a = a + 1 

[<FieldOnly>] // Should fail
let func3 (a, b) = a + b 

[<FieldOnly>] // Should fail
let func4 (a: int) : int = a + 1

[<FieldOnly>] // Should fail
let func5 a b = [ a; b ] 

[<FieldOnly>] // Should fail
let func6<'a> = "someTypedFunction" 

[<FieldOnly>] // Should fail
let func7<'a> (x : 'a) = "someTypedFunction2" 

[<FieldOnly>] // Should fail
let func8 = fun x -> x 

[<FieldOnly>]  // Should fail
let func9 = id

[<FieldOnly>] // Should fail
let __func10<'a> = false 

[<FieldOnly>] // Should fail
let __func11<'a> : bool  = false 

[<FieldOnly>] // Should fail
let (|Bool|_|) = function "true" -> Some true | "false" -> Some false | _ -> None 

[<FieldOnly>] // Should fail
[<return: Struct>]
let (|BoolExpr2|_|) = function "true" -> ValueSome true | "false" -> ValueSome false | _ -> ValueNone 

[<FieldOnly>]  // Should fail
let (|BoolExpr3|_|) x =
 match x with
 | "true" -> Some true 
 | "false" -> Some false 
 | _ -> None
 
[<FieldOnly>] // Should fail
[<return: Struct>]
let (|BoolExpr4|_|) x = 
 match x with
 | "true" -> ValueSome true 
 | "false" -> ValueSome false 
 | _ -> ValueNone

[<FieldOnly>] // Should fail
let rec func12() = 0 
and [<FieldOnly>] func13() = [] // Should fail

