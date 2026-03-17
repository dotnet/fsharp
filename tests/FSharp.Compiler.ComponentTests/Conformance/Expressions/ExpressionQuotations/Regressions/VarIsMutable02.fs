// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5534
// Make sure isMutable is set correctly for quoted mutables
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

let q = <@ let mutable x = 1 in if x = 1 then x <- 2 @>
let q1 = <@ let x = 1 in x @>

exit <| match q, q1 with
        |Let(v,e,b), Let(v1,e1,b1) -> if v.IsMutable && not v1.IsMutable then 0 else 1
        |_ -> 1
