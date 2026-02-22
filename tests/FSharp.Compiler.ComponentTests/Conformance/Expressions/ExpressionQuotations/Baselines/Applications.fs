// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let func (x : int) (y : int) = x + y
           let curriedFunc : int -> int = func 1
           let appFunc (f : int -> int) (x : int) = f (x + 1)
           appFunc curriedFunc 3 @>

let q' = Expr.Applications(Expr.Lambda(Var("x", typeof<int>), Expr.Lambda(Var("y", typeof<int>), Expr.Value(0))), [[Expr.Value(1)]; [Expr.Value(2)]])

let r1 = verify q (|Applications|_|) "Let (func, Lambda (x, Lambda (y, Call (None, op_Addition, [x, y]))),
     Let (curriedFunc, Application (func, Value (1)),
          Let (appFunc,
               Lambda (f,
                       Lambda (x,
                               Application (f,
                                            Call (None, op_Addition,
                                                  [x, Value (1)])))),
               Application (Application (appFunc, curriedFunc), Value (3)))))"
let r2 = verify q' (|Applications|_|) "Application (Application (Lambda (x, Lambda (y, Value (0))), Value (1)),
             Value (2))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
