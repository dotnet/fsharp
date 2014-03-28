// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(5,39-5,45)" id="FS0020">This expression should have type 'unit', but has type 'Async<'a>'\. Use 'ignore' to discard the result of the expression, or 'let' to bind the result to a name\.$</Expects>
let rec loop() = async { let x = 1 in loop() }
