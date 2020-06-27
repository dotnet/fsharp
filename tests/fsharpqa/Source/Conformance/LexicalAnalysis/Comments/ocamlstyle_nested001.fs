// #Regression #Conformance #LexicalAnalysis 
#light 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

let y1 = (* This is a comment with (**) nested *) 0
exit (if y1 = 0 then 0 else 1)
