// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(6,41-6,48)" id="FS0020">This expression should have type 'unit', but has type 'Async<'a>'\. Use 'ignore' to discard the result of the expression, or 'let' to bind the result to a name\.$</Expects>
//<Expects status="error" span="(6,50-6,52)" id="FS0001">This expression was expected to have type.    Async<'a>    .but here has type.    unit</Expects>
let rec loop2() = async.Delay(fun () -> loop2(); ()); 
