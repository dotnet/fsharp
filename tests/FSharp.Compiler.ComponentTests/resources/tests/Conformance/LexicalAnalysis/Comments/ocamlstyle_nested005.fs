// #Regression #Conformance #LexicalAnalysis 
#light 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

let y5= (* This is a comment with (***) nested *) 5

exit (if y5 = 5 then 0 else 1)
