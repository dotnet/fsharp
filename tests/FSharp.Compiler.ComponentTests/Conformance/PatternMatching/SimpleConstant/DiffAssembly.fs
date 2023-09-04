// #Conformance #PatternMatching #Constants 
#light



// Verify pattern matching against constants defined in a different assembly.

let testMatch x =
    match x with
    | System.Math.PI -> 1
    | _ -> 2

if testMatch System.Math.PI <> 1 then exit 1
if testMatch 0.0            <> 2 then exit 1

exit 0
