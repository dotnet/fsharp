// #Regression #Conformance #PatternMatching #Tuples 
#light

// Verify warning if all DU tags are matched with and
// a wildcard is included.
//<Expects id="FS0026" status="warning">This rule will never be matched</Expects>

type DU = A | B | C of int

let test = 
    function | A -> 1    | B -> 2    | C(0) -> 3 
             | C(x) -> x | _ -> failwith "Bummer"

let _ = test A
let _ = test B
let _ = test <| C(0)
let _ = test <| C(4)

exit 0
