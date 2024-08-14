// #Conformance #TypesAndModules 
// Type abbreviation
// The type nested in modules
//<Expects status="success"></Expects>
#light

module M1 = 
    module M2 = 
        module M3 =
              type TypeNestedInModules = | A = 1

// Q is a nice shortcut 
type Q = M1.M2.M3.TypeNestedInModules

let q = Q.A
let q' = M1.M2.M3.TypeNestedInModules.A

if q <> q' then failwith "Failed: 1"
