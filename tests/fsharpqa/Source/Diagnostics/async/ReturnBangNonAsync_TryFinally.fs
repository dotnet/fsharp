// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(7,11-7,12)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
async { try 
          return 1
        finally
          1 }  |> ignore
