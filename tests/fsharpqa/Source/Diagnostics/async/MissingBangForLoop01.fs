// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the ! for a loop
//<Expects status="error" span="(6,33-6,39)" id="FS0001">Type mismatch\. Expecting a.    'a    .but given a.    Async<'a>    .The resulting type would be infinite when unifying ''a' and 'Async<'a>'$</Expects>
let rec loop() = async { let x = 1 
                         return loop() 
                       }
