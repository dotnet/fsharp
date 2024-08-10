// This tests that AttributeTargets.Field is not allowed in class let function bindings

open System
open System.Diagnostics

[<AttributeUsage(AttributeTargets.Field)>]
type FieldOnlyAttribute() = 
   inherit Attribute()

type TestClass() =
   [<FieldOnly>]  // Should succeed (special exception when function can be lifted to a field)
   static let func1() = "someFunction"

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   static let rec func2() = "someFunction"

   [<FieldOnly>]// Should succeed (special exception when function can be lifted to a field)
   static let rec func3() = "someFunction" 
   and [<FieldOnly>] fun4() = "someFunction" // Should succeed (special exception when function can be lifted to a field)

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func5 () = "someFunction"

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func6 a = a + 1

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func7 (a, b) = a + b

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func8 (a: int) : int = a + 1

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func9 a b = [ a; b ]

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func10 = fun x -> x

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let func11 = id

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let (|Bool|_|) = function "true" -> Some true | "false" -> Some false | _ -> None

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   [<return: Struct>]
   let (|BoolExpr2|_|) = function "true" -> ValueSome true | "false" -> ValueSome false | _ -> ValueNone

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let (|BoolExpr3|_|) x =
    match x with
        | "true" -> Some true 
        | "false" -> Some false 
        | _ -> None

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   [<return: Struct>]
   let (|BoolExpr4|_|) x =
        match x with
        | "true" -> ValueSome true 
        | "false" -> ValueSome false 
        | _ -> ValueNone

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let rec func12() = 0
   and [<FieldOnly>] func13() = [] // Should succeed (special exception when function can be lifted to a field)

   [<FieldOnly>] // Should succeed (special exception when function can be lifted to a field)
   let rec func14() = 0