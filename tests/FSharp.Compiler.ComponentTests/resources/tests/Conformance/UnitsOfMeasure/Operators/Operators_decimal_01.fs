// #Regression #Conformance #UnitsOfMeasure #Operators 
// Regression tests for FSHARP1.0:3427
// The strategy here is just to make sure all the
// "reasonable" operators are implemented. We are
// not really looking at the results.
//<Expects status="success"></Expects>
module M

module WittDecimal =
    [<Measure>] type A
    type Ab = decimal<A>        // Type abbreviation
    let b : Ab = 100.0M<A>
    
    let _ = b + 2.0M<A>
    let _ = b - 2.0M<A>
    let _ = b * 2.0M
    let _ = b / 2.0M
    let _ = -b
    let _ = b < 2.0M<A>
    let _ = b <= 2.0M<A>
    let _ = b > 2.0M<A>
    let _ = b >= 2.0M<A>
    let _ = b <> 2.0M<A>
    let _ = max b b
    let _ = min b b
    let _ = abs b
    let _ = sign b
    let _ = b |> ignore
    let _ = ignore <| b
    let _ = ignore b
    let _ = box b
    let _ = hash b
    let _ = sizeof<Ab>
    let _ = typeof<Ab>
    let _ = typedefof<Ab>
    let _ = unbox b
    let _ = ref b
    let _ = decimal b



