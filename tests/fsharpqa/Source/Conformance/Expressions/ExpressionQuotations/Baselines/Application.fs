// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let f (g : int -> int) (x : int) = g (x + 1)
           f @>

let q' = Expr.Application(
            Expr.Lambda(
                Var("x", typeof<string>),
                Expr.Value(1)
            ),
            Expr.Value("arg1")
          )
          
let r1 = verify q (|Application|_|) "Let (f,
     Lambda (g,
             Lambda (x,
                     Application (g, Call (None, op_Addition, [x, Value (1)])))),
     f)"
let r2 = verify q' (|Application|_|) "Application (Lambda (x, Value (1)), Value (\"arg1\"))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
