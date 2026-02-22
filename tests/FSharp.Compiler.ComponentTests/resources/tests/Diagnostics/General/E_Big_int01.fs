// #Regression #Diagnostics 
// Regression test for FSHARP1.0:5860
// Big_int module removed from PowerPack (was deprecated previously)
//<Expects status="error" span="(7,6-7,13)" id="FS0039">The namespace or module 'Big_int' is not defined</Expects>
//<Expects status="error" span="(9,9-9,16)" id="FS0039">The value, namespace, type or module 'Big_int' is not defined</Expects>

open Big_int

let o = Big_int.big_int_of_int 1

exit <| if o.IsOne then 0 else 1
