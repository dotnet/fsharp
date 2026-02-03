// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let x = 1 in x @>
let q' = Expr.Var(Var("x", typeof<string>, true))

let r1 =
    match q with
    | Let (_, _, body) ->
        match body with
        | Microsoft.FSharp.Quotations.Patterns.Var x -> 0
        | _ -> 1
    | _ -> 2

let r2 = verify q' (|Var|_|) "x"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
