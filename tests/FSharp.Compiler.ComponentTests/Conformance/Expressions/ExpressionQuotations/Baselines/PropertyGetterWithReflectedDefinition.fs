// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape
open QuoteUtils

type T() =
    member this.x with [<ReflectedDefinition>] get() : int = this.x
                  and set v = this.x <- v
    member this.y with get() = this.y : int
                  and set v = this.y <- v

let q = <@ let t = T()
           t.x @>
let q' = <@ let t = T()
            t.x <- 2 @>
let q2 = <@ let t = T()
            t.y <- 2
            t.y @>

let hasReflectedDefinitionPropertyGetter quote =
    let found = ref false
    let rec traverse q = match q with
                         | PropertyGet (inst, pi, args) -> match pi with
                                                           | PropertyGetterWithReflectedDefinition e -> found := true
                                                           | _ -> ()
                                                           match inst with
                                                           | Some e -> Expr.PropertyGet(e, pi, args)
                                                           | None -> Expr.PropertyGet(pi, args)
                         | ShapeVar v -> Expr.Var v
                         | ShapeLambda (v,expr) -> Expr.Lambda (v, traverse expr)
                         | ShapeCombination (o, exprs) -> RebuildShapeCombination(o, List.map traverse exprs)
    (traverse quote, found)

// ensure the quotation matches and that the rebuilt quotation retained the ReflectedDefinition attribute
let q1, f1 = hasReflectedDefinitionPropertyGetter q
let r1 = match !f1 with
         | true -> let q2, f2 = hasReflectedDefinitionPropertyGetter q
                   match !f2 with
                   | true -> 0
                   | false -> 1
         | _ -> -1

let r1' = match hasReflectedDefinitionPropertyGetter q' with
          | _, f -> if !f = true then 1 else 0
let r2 = match hasReflectedDefinitionPropertyGetter q2 with
         | _, f -> if !f = true then 1 else 0

exit <| if r1 = 0 && r1' = 0 && r2 = 0 then 0 else 1
