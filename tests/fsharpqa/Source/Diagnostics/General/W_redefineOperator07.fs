// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3292
// Give a warning when the user attempts to redefine "=", "<", ">", ">=", "<=" or "<>" or define static members with these names
//<Expects id="FS0086" span="(8,6-8,7)" status="warning">The '&' operator should not normally be redefined\. Consider using a different operator name</Expects>
//<Expects id="FS0086" span="(9,8-9,9)" status="warning">The '&' operator should not normally be redefined\. Consider using a different operator name</Expects>
//<Expects id="FS0086" span="(11,20-11,21)" status="warning">The name '\(&\)' should not be used as a member name\. If defining a static member for use from other CLI languages then use the name 'op_Amp' instead\.$</Expects>
module M
let (&) x y = x + y
let f (&)  = 1 < 2
type C() =
    static member (&) (x:C,y:C) = true


