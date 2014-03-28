// #Regression #Conformance #Quotations 
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

// FSB 1332, ReflectedDefinition returning null should be allowed

module Repro = 

    [<ReflectedDefinition>]
    let a3 x = null
    
    let result = 
        match <@@ a3 @@> with 
        | Lambda(_,Call(_,minfo,_)) -> 
            match (Expr.TryGetReflectedDefinition minfo) with
            | Some(topExpr)-> "Resolved";
            | None         -> "Not Resolved";
        | expr -> "Not a top definition";

    printfn "input = %A"  <@ a3 @> 
    printfn "result = %s" result
    if result <> "Resolved" then exit 1
    exit 0
