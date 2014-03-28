// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

type t = | A of int | B of string
let q = <@ let m = match A(1) with | A x -> 0 | B x -> 1 in () @>
let uci = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<t>)
let q' = Expr.UnionCaseTest(Expr.NewUnionCase(uci.[0], [Expr.Value(1)]), uci.[0]) 
let r1 = verify q (|UnionCaseTest|_|) "Let (m,
     Let (matchValue, NewUnionCase (A, Value (1)),
          IfThenElse (UnionCaseTest (matchValue, B),
                      Let (x, PropertyGet (Some (matchValue), Item, []),
                           Value (1)),
                      Let (x, PropertyGet (Some (matchValue), Item, []),
                           Value (0)))), Value (<null>))"
                                
let r2 = verify q' (|UnionCaseTest|_|) "UnionCaseTest (NewUnionCase (A, Value (1)), A)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
