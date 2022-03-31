// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2791
// Make sure we can parse arrays of dimensioned numbers without needing a a space between > and |]
//
#light

[<Measure>]
type m

let l1 = [|10.0<m>|]         // ok
let l2 = [|10.0<m> |]        // ok

// This was not really specific to Units of Measure, we while we are here
// we check that care too.
let p1 = [|Array.empty<float>|]     // ok
let p2 = [|Array.empty<float> |]    // ok
printfn "Finished"
