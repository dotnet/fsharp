// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let f x y = x + y
           let g x = f x 3
           g 2 @>
let q' = Expr.Lambda(Var("x", typeof<int -> int>), Expr.Lambda(Var("y", typeof<int>), Expr.Value(0)))

let r1 = verify q (|Lambdas|_|) "Let (f, Lambda (x, Lambda (y, Call (None, op_Addition, [x, y]))),
     Let (g, Lambda (x, Application (Application (f, x), Value (3))),
          Application (g, Value (2))))"
let r2 = verify q' (|Lambdas|_|) "Lambda (x, Lambda (y, Value (0)))"

exit <| if r1 = 0 && r2 = 0 then 0 else 1
