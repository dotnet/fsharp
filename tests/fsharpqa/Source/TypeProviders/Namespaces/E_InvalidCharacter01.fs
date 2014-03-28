// #Conformance #TypeProvider #Regression
// Regression test for DEV11:150508 - error spans when opening a provided namespace completely wrong
//
//<Expects status="error" span="(7,6-7,13)" id="FS0883">Invalid namespace, module, type or union case name$</Expects>
//<Expects status="error" span="(8,9-8,12)" id="FS3000">Character '\+' is not allowed in provided namespace name 'A\+B'$</Expects>
module M
open ``A+B``
let k = T.M(1,2)


