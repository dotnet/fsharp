// #Regression #Conformance #BasicGrammarElements #Operators
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
// Note: as of 11/1, the usage of ~ is restricted to very specific cases. 
//<Expects status="error" span="(14,6-14,9)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(15,6-15,10)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(17,25-17,26)" id="FS1208">Invalid prefix operator$</Expects>
//<Expects status="error" span="(18,27-18,28)" id="FS1208">Invalid prefix operator$</Expects>

module M

let (~!!) x y = List.item y x
let (~!!!) x y = List.item y x

let err = [1 .. 10] ~!! 2            // error
let err2 = [1 .. 10] ~!!! 2          // error

exit 1
