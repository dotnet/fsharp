// #Conformance #Quotations 
open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let m = (match new Object() with | :? int -> 1 | _ -> 0) |> ignore
        1
let q = <@ let m = match new Object() with | :? int -> 1 | _ -> 0 in m @>
let q' = Expr.Let(Var("x", typeof<bool>), Expr.TypeTest(Expr.Value(0), typeof<int>), Expr.Value(0))
let r1 = verify q (|TypeTest|_|) "Let (m,
     Let (matchValue, NewObject (Object),
          IfThenElse (TypeTest (Int32, matchValue), Value (1), Value (0))), m)"
let r2 = verify q' (|TypeTest|_|) "Let (x, TypeTest (Int32, Value (0)), Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
