// #Regression #Diagnostics #RequiresPowerPack 
// Regression test for FSHARP1.0:5860
// Num module removed from PowerPack
//<Expects status="error" span="(6,6-6,9)" id="FS0039">The namespace or module 'Num' is not defined$</Expects>

open Num

exit 0
