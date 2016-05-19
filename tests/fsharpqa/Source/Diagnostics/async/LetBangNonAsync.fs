// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(4,18-4,19)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'int'</Expects>
async { let! x = 1 in return 1 } |> ignore
