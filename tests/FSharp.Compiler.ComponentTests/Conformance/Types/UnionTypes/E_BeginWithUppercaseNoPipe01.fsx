// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:3741
//<Expects id="FS0053" span="(11,18-11,19)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
//<Expects id="FS0053" span="(12,18-12,19)" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
#light

module ``*`` = 

    type DU  =   A  // ok - uppercase
    type DU2 = | A  // ok - uppercase!
    type du =    a  // error - lowercase (before the fix for 3741, this used to be ok!)
    type du2 = | a  // error - lowercase

let w = ``*``.DU.A


