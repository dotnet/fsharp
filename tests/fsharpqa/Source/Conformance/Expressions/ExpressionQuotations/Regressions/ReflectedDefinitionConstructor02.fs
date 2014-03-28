// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:6159
// Couldn't quote constructor

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type Foo[<ReflectedDefinition>]() =
    let x = 2
    member this.Test y = x + y

let q1 = <@ Foo() @>
let f q =
    let ci = match q with | Patterns.NewObject(ci , args) -> Some(ci) | _ -> None
    Expr.TryGetReflectedDefinition ci.Value

let r1 = f q1

exit <| match r1 with | Some(_) -> 0 | _ -> 1
