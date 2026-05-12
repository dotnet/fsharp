// #Regression #Conformance #PatternMatching #TypeTests 


// Verify error associated with doing a dynamic type
// test on a variable type.



// Error, x has type 'a and cannot be used in a dynamic type test.
let f x = 
    match x with
    | :? obj as o -> true
    | _ -> false

exit 1
