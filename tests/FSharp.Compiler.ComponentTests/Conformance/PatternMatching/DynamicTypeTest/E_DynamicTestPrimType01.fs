// #Regression #Conformance #PatternMatching #TypeTests 
#light

// Verify error when using a dynamic type test on 
// type without any proper sub types.

//<Expects id="FS0016" status="error">The type 'int' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion</Expects>

let test (x : int) =
    match x with
    | :? float -> false
    | _ -> false

exit 1
