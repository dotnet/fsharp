// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

#light 

let x (**) = (**) 1

exit (if x = 1 then 0 else 1)
