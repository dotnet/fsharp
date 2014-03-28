// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape
open QuoteUtils

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

// ensure the quotation matches and that the rebuilt quotation retained the ReflectedDefinition attribute, and check the quote's shape
let doubleCheckReflectedDefinition (quote : Expr) (shapeString : string) =
    let qr, f = hasMethodWithReflectedDefinition quote
    let res = 
        match !f with
        | true -> let qr2, f2 = hasMethodWithReflectedDefinition qr
                  if !f2 then 0 else 1
        | false -> -1
        
    let shapeResult = checkQuote quote shapeString
    let shapeResult2 = checkQuote qr shapeString
    
    res + shapeResult + shapeResult2
    
// test type with extension members with reflected definitions
type System.String with
    member this.Foo = 1
    
    [<ReflectedDefinition>]    
    member this.Bar = 2
    
    [<ReflectedDefinition>]
    member this.Ram x = this.Bar * x

let q1 = <@ "a".Foo @>
let q2 = <@ "a".Bar @>
let q3 = <@ "a".Ram 3 @>

let r1 = doubleCheckReflectedDefinition q1 "Call (None, String.get_Foo, [Value (\"a\")])"
let r2 = doubleCheckReflectedDefinition q2 "Call (None, String.get_Bar, [Value (\"a\")])"
let r3 = doubleCheckReflectedDefinition q3 "Call (None, String.Ram, [Value (\"a\"), Value (3)])"

exit <| match r1, r2, r3 with
        | -1, 0, 0 -> 0
        | _, _, _ -> 1
