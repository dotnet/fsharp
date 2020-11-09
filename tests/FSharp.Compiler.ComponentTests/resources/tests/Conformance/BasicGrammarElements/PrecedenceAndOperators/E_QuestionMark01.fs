// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression test for FSHARP1.0:2541
// Prefix only operators: !OP, ?OP, ~OP
// Try to use !OP as an infix-op is an error
// ?OP ~OP
//<Expects status="error" span="(12,6-12,9)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(13,6-13,10)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(15,25-15,26)" id="FS1208">Invalid prefix operator</Expects>
//<Expects status="error" span="(16,27-16,28)" id="FS1208">Invalid prefix operator</Expects>
module M

let (?!!) x y = List.item y x         // error
let (?!!!) x y = List.item y x        // error

let err = [1 .. 10] ?!! 2            // error
let err2 = [1 .. 10] ?!!! 2          // error

exit 1
