// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(8,35-8,42)" id="FS0020">This expression should have type 'unit', but has type 'Async<'a>'\. Use 'ignore' to discard the result of the expression, or 'let' to bind the result to a name\.$</Expects>
//<Expects status="error" span="(8,44-8,46)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'unit'</Expects>
// Note: interestingly, this looks much better if a method call is not used
let delay x = async.Delay x
let rec loop3() = delay(fun () -> loop3(); ()); 
