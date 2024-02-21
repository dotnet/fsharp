
open System

[<AttributeUsage(AttributeTargets.Field)>]
type FieldOnlyAttribute() = 
   inherit Attribute()

[<FieldOnly>]
let someValue = "someValue"

[<FieldOnly>]
let i, j, k = (1, 2, 3) // Should fail

[<FieldOnly>]
let someRecLetBoundValue = nameof(FieldOnlyAttribute)

[<FieldOnly>]
let rec someRecLetBoundValue2 = nameof(someRecLetBoundValue2)

[<FieldOnly>]
let ``someValue2`` = "someValue"

[<FieldOnly>]
let someFunction () = "someFunction"

[<FieldOnly>]
let someFunction2 a = a + 1

[<FieldOnly>]
let someFunction3 (a, b) = a + b

[<FieldOnly>]
let someFunction4 (a: int) : int = a + 1

[<FieldOnly>]
let makeList a b = [ a; b ]

[<FieldOnly>]
let someTypedFunction<'a> = "someTypedFunction"

[<FieldOnly>]
let someTypedFunction2<'a> (x : 'a) = "someTypedFunction2"

[<FieldOnly>]
let someTypedFunction3 = fun x -> x

[<FieldOnly>]
let someTypedFunction4 = id

[<FieldOnly>]
let __someTypedFunction5<'a> = false

[<FieldOnly>]
let __someTypedFunction6<'a> : bool  = false

type TestConst =
 | Bool of bool

[<NoComparison>]
type TestExpr =
 | Const of TestConst * Type * int
 | Var of string * Type * int

[<FieldOnly>]
let (|BoolExpr|_|) =
 function
 | TestExpr.Const (TestConst.Bool b, _, _) -> Some b
 | _ -> None

[<FieldOnly>]
[<return: Struct>]
let (|BoolExpr2|_|) =
 function
 | TestExpr.Const(TestConst.Bool b1, _, _) -> ValueSome b1
 | _ -> ValueNone

[<FieldOnly>]
let (|BoolExpr3|_|) x =
 match x with
 | TestExpr.Const (TestConst.Bool b, _, _) -> Some b
 | _ -> None

[<FieldOnly>]
[<return: Struct>]
let (|BoolExpr4|_|) x =
 match x with
 | TestExpr.Const(TestConst.Bool b1, _, _) -> ValueSome b1
 | _ -> ValueNone

let private dangling (target: TestExpr -> TestExpr voption) =
 function
 | TestExpr.Const (TestConst.Bool _, _, _) as b -> ValueSome b
 | _ -> ValueNone

[<FieldOnly>]
[<return: Struct>]
let (|IfThen|_|) =
 dangling (function
     | TestExpr.Const(TestConst.Bool b1, _, _) as expr -> ValueSome expr
     | _ -> ValueNone)

[<FieldOnly>]
let rec f = 0
and [<FieldOnly>] g() = []

[<FieldOnly>]
let rec f1 = 0
and [<FieldOnly>] g2() = []

[<FieldOnly>]
let (a :: _) = []

[<FieldOnly>]
let (d, e) as foo = 1, 2

[<FieldOnly>]
let 1 = 0

type X = { X: int }

[<FieldOnly>]
let { X = _ } = { X = 1 }