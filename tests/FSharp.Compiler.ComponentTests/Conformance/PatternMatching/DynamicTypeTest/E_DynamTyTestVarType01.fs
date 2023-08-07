// #Regression #Conformance #PatternMatching #TypeTests 
#light

// Verify error associated with doing a dynamic type
// test on a variable type.

//<Expects id="FS0008" status="error">This runtime coercion or type test from type</Expects>

// Error, x has type 'a and cannot be used in a dynamic type test.
let f x = 
    match x with
    | :? obj as o -> true
    | _ -> false

exit 1
