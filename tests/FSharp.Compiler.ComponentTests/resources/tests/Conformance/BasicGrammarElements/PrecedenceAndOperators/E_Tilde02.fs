// #Regression #Conformance #BasicGrammarElements #Operators
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
//<Expects status="error" span="(12,6-12,9)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(13,6-13,10)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(15,15-15,22)" id="FS1208">Invalid prefix operator$</Expects>
//<Expects status="error" span="(16,16-16,17)" id="FS1208">Invalid prefix operator$</Expects>
module M

let (~!!) x y = List.item y x;;
let (~!!!) x y = List.item x y

let ok1 = ~!! [1..10] 2        // ok
let ok2 = ~!!! 3 [1..10]       // ok

(if ok1 = 3 && ok2 =4 then 0 else 1) |> exit

