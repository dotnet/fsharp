// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2804
// Make sure we don't emit ?. (notice that the error message changed a bit since the bug was opened)
//<Expects status="notin">\?\.</Expects>
//<Expects id="FS0001" status="error"></Expects>

let f (x : int list)  = int32 (x>>32)

exit 1
