// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape
open QuoteUtils

[<ReflectedDefinition>]
let rf x = x + 1
let nrf x = x + 1

let q = <@ rf 1 @>
let q' = <@ nrf 1 @>

let hasMethodWithReflectedDefinition quote =
    let found = ref false
    let rec traverse q = match q with
                         | Call (inst, mi, args) -> match mi with
                                                           | MethodWithReflectedDefinition e -> found := true
                                                           | _ -> ()
                                                    match inst with
                                                    | Some e -> Expr.Call(e, mi, args)
                                                    | None -> Expr.Call(mi, args)
                         | ShapeVar v -> Expr.Var v
                         | ShapeLambda (v,expr) -> Expr.Lambda (v, traverse expr)
                         | ShapeCombination (o, exprs) -> RebuildShapeCombination(o, List.map traverse exprs)
    (traverse quote, found)

// ensure the quotation matches and that the rebuilt quotation retained the ReflectedDefinition attribute
let q1, f1 = hasMethodWithReflectedDefinition q
let r1 = match !f1 with
         | true -> let q2, f2 = hasMethodWithReflectedDefinition q1
                   match !f2 with
                   | true -> 0
                   | false -> 1
         | _ -> -1

let r2 = match hasMethodWithReflectedDefinition q' with
         | _, f -> if !f = true then 1 else 0

exit <| if r1 = 0 && r2 = 0 then 0 else 1
