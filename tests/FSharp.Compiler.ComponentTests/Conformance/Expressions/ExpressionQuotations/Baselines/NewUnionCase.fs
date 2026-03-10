// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

type t = | A of int | B of string
let q = <@ let x, y = (A(1), B("1")) in x @>
     
let uci = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<t>)
let q' = Expr.NewUnionCase(uci.[0], [Expr.Value(1)])

let r1 = verify q (|NewUnionCase|_|) "Let (patternInput,
     NewTuple (NewUnionCase (A, Value (1)), NewUnionCase (B, Value (\"1\"))),
     Let (y, TupleGet (patternInput, 1), Let (x, TupleGet (patternInput, 0), x)))"
let r2 = verify q' (|NewUnionCase|_|) "NewUnionCase (A, Value (1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
