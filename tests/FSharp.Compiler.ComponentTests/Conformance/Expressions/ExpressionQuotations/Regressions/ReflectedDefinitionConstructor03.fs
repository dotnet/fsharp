// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5934 
// ReflectedDefinitionAttribute on constructors wasn't working

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.ExprShape

type MyClassWithFields [<ReflectedDefinition>]() = 
    let x = 12
    let y = x
    let w = x // note this variable is not used in any method and becomes local to the constructor

    [<ReflectedDefinition>]
    member this.Bar z = x + z + y
    
let mutable result = 0

let rec traverse q =
    match q with
    | ShapeVar v -> 
        // ensure the this variable used in the constructor matches the global
        if v.Name = "this" then
            let th = Var.Global("this",typeof<MyClassWithFields>)
            match th with
            | v when v <> th -> failwith "\"this\" pointer didn't match"
            | _ -> result <- result + 1
        Expr.Var(v)
    | ShapeLambda (v,e) -> Expr.Lambda(v, traverse e)
    | ShapeCombination (o,l) -> RebuildShapeCombination (o, (List.map traverse l))
    
let q = Expr.TryGetReflectedDefinition (typeof<MyClassWithFields>.GetConstructors().[0])

try
    traverse q.Value |> ignore
with
    | _ -> result <- 1
    
exit <| if result = 4 then 0 else 1
