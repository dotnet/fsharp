// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
//<Expects status="error" span="(11,6-11,10)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(13,15-13,22)" id="FS1208">Invalid prefix operator$</Expects>
//<Expects status="error" span="(14,16-14,17)" id="FS1208">Invalid prefix operator$</Expects>

module M
let (?!!!) x y = List.item x y

let ok1 = ?!! [1..10] 2
let ok2 = ?!!! 3 [1..10]

(if ok1 = 3 && ok2 = 4 then 0 else 1) |> exit
