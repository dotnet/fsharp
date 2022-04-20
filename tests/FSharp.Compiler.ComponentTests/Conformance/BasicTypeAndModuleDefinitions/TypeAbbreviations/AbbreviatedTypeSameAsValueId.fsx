// #Conformance #TypesAndModules 
// Type abbreviation
// Abbreviated type and value identifier do no conflict
//<Expects status="success"></Expects>

let T = null
type T = System.Numerics.BigInteger       // ok - no conflict

