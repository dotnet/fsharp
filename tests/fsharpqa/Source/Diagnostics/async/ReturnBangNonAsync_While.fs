// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
//<Expects status="error" span="(6,19-6,20)" id="FS0001">This expression was expected to have type.    'Async<unit>'    .but here has type.    'int'</Expects>
async { while true do 
          let x = 1 
          return! 1 } |> ignore

