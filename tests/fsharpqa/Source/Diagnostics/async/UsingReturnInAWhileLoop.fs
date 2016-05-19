// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// less common mistake: using "return" in a while loop
//<Expects status="error" span="(7,18-7,19)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>

async { while true do 
          return 1 } |> ignore
