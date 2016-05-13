// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(4,17-4,18)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'int'</Expects>
async { return! 1 } |> ignore
