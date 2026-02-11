// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5934 
// ReflectedDefinitionAttribute on constructors wasn't working

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type Foo() =
    [<ReflectedDefinition>]
    new (x:int) = Foo()
    member this.Test = 1

type Foo<'T>() =
    [<ReflectedDefinition>]
    new (x:int) = Foo<'T>()
    member this.Test = 1
    
let q1 = <@ Foo(1) @>
let q2 = <@ Foo() @>
let q3 = <@ Foo<int>(3) @>

let f q =
    let ci = match q with | Patterns.NewObject(ci , args) -> Some(ci) | _ -> None
    Expr.TryGetReflectedDefinition ci.Value

let r1 = f q1
let r2 = f q2
let r3 = f q3

exit <| match r1,r2,r3 with | Some(_), None, Some(_) -> 0 | _, _,  _ -> 1
