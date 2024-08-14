// #Conformance #TypesAndModules #Unions  
//<Expects id="FS1219" status="error">The union case named 'Tags' conflicts with the generated type 'Tags'</Expects>
// Regression test for Bug 6308
type BigUnion1 = 
    | Case0
    | Tags of bool
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

exit 1