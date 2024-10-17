// #Regression #Conformance #PatternMatching #TypeTests 
#light

// Verify error when using a dynamic type test on 
// type without any proper sub types.



let test (x : int) =
    match x with
    | :? float -> false
    | _ -> false

exit 1
