// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(4,9)" id="FS0588">Block following this 'let' is unfinished\. Expect an expression\.$</Expects>
let x = let y = 2;;
exit 1;;
