// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let rec f x = if x = 0 then 0 else f (x-1) 
           f 0 @>
let q' = Expr.LetRecursive([(Var("f", typeof<int->int>), Expr.Lambda(Var("x", typeof<int>), Expr.Value(1)))], Expr.Value(0))
let r1 = verify q (|LetRecursive|_|) "LetRecursive ([(f,Lambda (x,
                          IfThenElse (Call (None, op_Equality, [x, Value (0)]),
                                      Value (0),
                                      Application (f,
                                                   Call (None, op_Subtraction,
                                                         [x, Value (1)])))))],
              Application (f, Value (0)))"
let r2 = verify q' (|LetRecursive|_|) "LetRecursive ([(f,Lambda (x, Value (1)))], Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
