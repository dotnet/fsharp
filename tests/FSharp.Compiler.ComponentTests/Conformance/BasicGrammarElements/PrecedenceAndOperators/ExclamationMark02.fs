// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
//<Expects status="success"></Expects>



let (!!) x y = List.nth x y;;
let (!!!) x y = List.nth y x

let ok1 = !! [1..10] 2        // ok
let ok2 = !!! 3 [1..10]       // ok

(if ok1 = 3 && ok2 = 4 then 0 else 1) |> exit

