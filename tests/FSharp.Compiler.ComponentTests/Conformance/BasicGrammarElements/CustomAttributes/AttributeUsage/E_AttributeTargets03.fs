
open System

[<AttributeUsage(AttributeTargets.Method)>]
type MethodOnlyAttribute() = 
   inherit Attribute()

[<MethodOnly>]
let someValue = "someValue" // Should fail

[<MethodOnly>]
let i, j, k = (1, 2, 3) // Should fail

[<MethodOnly>]
let someRecLetBoundValue = nameof(MethodOnlyAttribute) // Should fail

[<MethodOnly>]
let rec someRecLetBoundValue2 = nameof(someRecLetBoundValue2) // Should fail

[<MethodOnly>]
let ``someValue2`` = "someValue" // Should fail

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

type TestConst =
 | Bool of bool

[<NoComparison>]
type TestExpr =
 | Const of TestConst * Type * int
 | Var of string * Type * int

[<MethodOnly>]
let (|BoolExpr|_|) =
 function
 | TestExpr.Const (TestConst.Bool b, _, _) -> Some b
 | _ -> None

[<MethodOnly>]
[<return: Struct>]
let (|BoolExpr2|_|) =
 function
 | TestExpr.Const(TestConst.Bool b1, _, _) -> ValueSome b1
 | _ -> ValueNone

[<MethodOnly>]
let (|BoolExpr3|_|) x =
 match x with
 | TestExpr.Const (TestConst.Bool b, _, _) -> Some b
 | _ -> None

[<MethodOnly>]
[<return: Struct>]
let (|BoolExpr4|_|) x =
 match x with
 | TestExpr.Const(TestConst.Bool b1, _, _) -> ValueSome b1
 | _ -> ValueNone

let private dangling (target: TestExpr -> TestExpr voption) =
 function
 | TestExpr.Const (TestConst.Bool _, _, _) as b -> ValueSome b
 | _ -> ValueNone

[<MethodOnly>]
[<return: Struct>]
let (|IfThen|_|) =
 dangling (function
     | TestExpr.Const(TestConst.Bool b1, _, _) as expr -> ValueSome expr
     | _ -> ValueNone)

[<MethodOnly>]
let rec f = 0
and [<MethodOnly>] g() = []

[<MethodOnly>]
let rec f1 = 0
and [<MethodOnly>] g2() = []

[<MethodOnly>]
let (a :: _) = []

[<MethodOnly>]
let (d, e) as foo = 1, 2

[<MethodOnly>]
let 1 = 0

type X = { X: int }

[<MethodOnly>]
let { X = _ } = { X = 1 }
