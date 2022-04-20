// #Conformance #TypesAndModules #Exceptions 
// An exception definition no longer generates a type with name idException
// (this used to be the case until Dev10 Beta1, but not anymore)
// In this case we check and see what happens when such a type already exist
//<Expects status="success"></Expects>

exception E of string
type EException = | EE

let _ = E("")     // exception
let _ = EE        // DU
