// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:3741


#light

module ``*`` = 

    type DU  =   A  // ok - uppercase
    type DU2 = | A  // ok - uppercase!
    type du =    a  // error - lowercase (before the fix for 3741, this used to be ok!)
    type du2 = | a  // error - lowercase

let w = ``*``.DU.A


