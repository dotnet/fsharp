// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
//<Expects id="FS0003" span="(14,11-14,20)" status="error">This value is not a function and cannot be applied</Expects>
//<Expects id="FS0003" span="(15,12-15,21)" status="error">This value is not a function and cannot be applied</Expects>

module M

let (!!) x y = List.item y x;;
let (!!!) x y = List.item y x

let err = [1 .. 10] !! 2            // error
let err2 = [1 .. 10] !!! 2          // error

failwith "Failed: 1"

