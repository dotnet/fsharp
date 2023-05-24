// #Regression #Conformance #LexicalAnalysis 

// Regression test for FSHARP1.0:1561
// Since OCaml-style comments are now gone, this is going to be a negative test
//<Expects id="FS0010" span="(12,47)" status="error">Unexpected string literal in binding\. Expected incomplete structured construct at or before this point or other token\.$</Expects>
//<Expects status="error" span="(10,1)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>
//<Expects id="FS3156" span="(14,19)" status="error">Unexpected token '*' or incomplete expression</Expects>
//<Expects id="FS0010" span="(14,20)" status="error">Unexpected symbol '\)' in implementation file</Expects>

let y7a = (** This is a comment with "(***)" *) 1 (**) + 2"

let y7b = (* * This is a comment with "(* * *)" *) 1 (*  *) + 2

exit (if( y7a = " *) 1 (**) + 2" && y7b = 3) then 0 else 1)

let y6 = (** This is a comment with (***) 6 (**)

if y6 <> 6 then exit 1

exit 0


