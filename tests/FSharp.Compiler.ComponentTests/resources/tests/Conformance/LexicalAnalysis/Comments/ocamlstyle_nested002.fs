// #Regression #Conformance #LexicalAnalysis 
#light 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

let y2 = (* This is a comment with * nested *) 2
exit (if y2 = 2 then 0 else 1)
