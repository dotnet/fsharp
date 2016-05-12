// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(5,18-5,19)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
async { for x in [1;2] do 
          return 1 }  |> ignore
