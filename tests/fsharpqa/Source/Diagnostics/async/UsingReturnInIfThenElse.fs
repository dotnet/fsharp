// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// less common mistake: using "return" in an if/then/else
//<Expects status="error" span="(6,9-6,21)" id="FS0193">Type constraint mismatch\. The type .    'Async<unit>'    .is not compatible with type.    'Async<int>'$</Expects>
//
async { if true then 
          return 1 } |> ignore
