// #Regression #Conformance #LexicalAnalysis #Operators 
// Regression test for FSHARP1.0:4994
// We could not define operator >.
//<Expects status="error" span="(9,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(10,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(12,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
//<Expects status="error" span="(13,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>

let ( ~>. ) x y = x + y
let ( ~<. ) x y = x + y

let ( ~!>. ) x y = x + y
let ( ~!<. ) x y = x + y
