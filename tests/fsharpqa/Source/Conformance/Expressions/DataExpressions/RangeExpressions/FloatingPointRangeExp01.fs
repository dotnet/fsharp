// #Regression #Conformance #DataExpressions 
#light

// FSB 1005, float comprehensions of form "x0 .. dx .. x1" suffer rounding errors
// After changes in BigInt, this is no longer a warning.
//<Expects status="notin">Floating point ranges are experimental and may be deprecated in a future release</Expects>

let floatingPointRange = [0.0 .. 0.01 .. 2.0]
let lastNum, sndToLast = floatingPointRange 
                         |> List.rev
                         |> function fst :: snd :: rest -> (fst, snd)

if lastNum   <> 2.0  then exit 1
if sndToLast <> 1.99 then exit 1

exit 0
