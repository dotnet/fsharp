// #Conformance #PatternMatching 
#light

// Verify that if the when guard fails, that pattern is not matched.

let complexXOR x y = 
    match x with
    | true when not y -> true
    | false when y -> true
    | true when y -> false
    | false when not y -> false
    | _ -> failwith "not possible"

let simpleXOR x y = x <> y

let runTest (x, y) = if complexXOR x y <> simpleXOR x y then exit 1
let testSuite = [(true, true); (true, false); (false, true); (false, false)]
List.iter runTest testSuite

exit 0
