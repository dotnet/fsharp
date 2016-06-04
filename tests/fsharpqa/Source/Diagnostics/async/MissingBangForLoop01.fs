// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the ! for a loop
//<Expects status="error" span="(6,33-6,39)" id="FS0001">Type mismatch\. Expecting a.+''a'.+but given a.+'Async<'a>'.*The types ''a' and 'Async<'a>' cannot be unified.</Expects>
let rec loop() = async { let x = 1 
                         return loop() 
                       }
