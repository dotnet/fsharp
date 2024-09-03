// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP



module M

let (!!) x y = List.item y x;;
let (!!!) x y = List.item y x

let err = [1 .. 10] !! 2            // error
let err2 = [1 .. 10] !!! 2          // error

failwith "Failed: 1"

