// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1729
// Notice that the bug was in the IDE, but a compiler test is equally useful.
//<Expects id="FS0886" span="(10,11-10,13)" status="error">This is not a valid value for an enumeration literal</Expects>
//<Expects id="FS0886" span="(14,11-14,13)" status="error">This is not a valid value for an enumeration literal</Expects>
#light

// Shouldn't work    
type EnumOfBigInt =
    | A = 0I
    | B = 0I

type EnumOfNatNum =
    | A = 0N
    | B = 0N
