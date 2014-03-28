// #Regression #Diagnostics #Async 
// Regression tests for FSHARP1.0:4394
// common mistake: forgetting the ! for a loop
// Note: Desugared form of MissingBangForLoop01.fs
//<Expects status="error" span="(7,54-7,61)" id="FS0001">Type mismatch\. Expecting a.    'a    .but given a.    Async<'a>    .The resulting type would be infinite when unifying ''a' and 'Async<'a>'$</Expects>

let rec loop2() = async.Delay(fun () -> async.Return(loop2())) 
