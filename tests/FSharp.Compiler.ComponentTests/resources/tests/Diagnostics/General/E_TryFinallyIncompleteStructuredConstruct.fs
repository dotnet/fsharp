// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2520
// The verification (to to speak) is to make sure we emit _at_least_ the error message ("missing qualification after '.')
// Before the fix apparently, we were not emitting it.
//<Expects status="error" span="(9,6-9,7)"   id="FS0599">Missing qualification after '\.'</Expects>

let f() =
    let x = 3
    x.                        // <- error
    try
        ()
    finally
        ()
