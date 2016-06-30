// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(7,18-7,20)" id="FS0001">This expression was expected to have type.    'int'    .but here has type.    'unit'</Expects>
async { try 
          return 1
        with  _ -> 
          return () }  |> ignore
