// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify active patterns outside of match statement
// where active pattern defines a function. (Regression.)

let (|ApplyTwice|) (x : int -> int) =  x >> x

let (ApplyTwice result) = fun x -> x * x

if result 2 <> 2 * 2 * 2 * 2 then exit 1

exit 0
