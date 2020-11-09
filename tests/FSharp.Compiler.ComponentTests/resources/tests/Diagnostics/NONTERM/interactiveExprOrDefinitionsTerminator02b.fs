// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error">Unexpected symbol ',' in lambda expression.</Expects>

fun x, y -> x < y
