// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// less common mistake: using "return" in an if/then/else
//<Expects status="error" span="(6,9-6,21)" id="FS0001">Type mismatch\. Expecting a.    'Async<int>'    .but given a.    'Async<unit>'    .The type 'int' does not match the type 'unit'$</Expects>
//
async { if true then 
          return 1 } |> ignore
