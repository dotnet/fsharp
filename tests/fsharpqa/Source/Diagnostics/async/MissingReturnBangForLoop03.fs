// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(6,41-6,48)" id="FS0020">This expression has a value of type 'Async<'a>' that is implicitly ignored\. Use the 'ignore' function to discard this value explicitly, e\.g\. 'expr \|> ignore', or bind it to a name to refer to it later, e\.g\. 'let result = expr'\.$</Expects>
//<Expects status="error" span="(6,50-6,52)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'unit'</Expects>
let rec loop2() = async.Delay(fun () -> loop2(); ()); 
