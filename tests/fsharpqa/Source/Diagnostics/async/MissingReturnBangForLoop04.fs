// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the return! For a loop
//<Expects status="error" span="(8,35-8,42)" id="FS0020">The result of this expression is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
//<Expects status="error" span="(8,44-8,46)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'unit'</Expects>
// Note: interestingly, this looks much better if a method call is not used
let delay x = async.Delay x
let rec loop3() = delay(fun () -> loop3(); ()); 
