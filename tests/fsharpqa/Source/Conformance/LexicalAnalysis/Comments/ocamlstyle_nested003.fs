// #Regression #Conformance #LexicalAnalysis 
#light 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

let y3 = (* This is a comment with (* *) nested *) 3

exit (if y3 = 3 then 0 else 1)
