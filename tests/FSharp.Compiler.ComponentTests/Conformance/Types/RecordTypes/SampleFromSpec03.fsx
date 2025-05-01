// #Conformance #TypesAndModules #Records 
// Sample found on the spec.
// Section 9.2 (3rd code snippet)
//<Expects status="success"></Expects>
#light

type R = { dx : int; dy: int }
let f x = x.dx                          // x is inferred to have type R

[<NoEquality; NoComparison>]
type D = | E of (R -> int)
let _ = E(f)                            // the fact we compile just fine verifies that f is R -> int
