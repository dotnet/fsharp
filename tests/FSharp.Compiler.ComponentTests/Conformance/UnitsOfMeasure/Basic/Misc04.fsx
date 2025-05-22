// #Regression #Conformance #UnitsOfMeasure 
#light 

// Regression for FSB 3472, apply the type equations float<1> = float, float32<1> = float, decimal<1> = decimal when computing all predicates on types

let a : decimal<1> = ceil 4.4M<1> 
let b : decimal<1> = floor 4.4M<1> 
let c : decimal<1> = round 4.4M<1>

if a <> 5.0M then exit 1
if b <> 4.0M then exit 1
if c <> 4.0M then exit 1

exit 0
