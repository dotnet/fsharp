// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(5,39-5,45)" id="FS0020">The result of this expression is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
let rec loop() = async { let x = 1 in loop() }
