// #Conformance #TypesAndModules #Unions 
// RegressionTest for bug 6308 
//<Expects status="error" id="FS0023" span="(19,14-19,17)">The member 'Tag' can not be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module</Expects>

[<DefaultAugmentation(false)>]
type BigUnion2 = 
    | Case0
    | Case11 of bool
    | Case22 of bool
    | Case3 of bool
    | Case4 of bool
    | Case5 of bool
    | Case6 of bool
    | Case7 of bool
    | Case8 of bool
    | Case9 of bool
    | Case10 of bool
    | Case12 of string*int
    member x.Tag = 0
exit 1