// #Regression #Conformance #LexicalAnalysis 
#light 
// Regression test for FSHARP1.0:1561
// Verify that (**) does not leave the lexer in a comment state
//<Expects status="success"></Expects>

let y4 = (* This is a comment with (** **) nested *) 4
ignore (if y4 = 4 then 0 else 1)
