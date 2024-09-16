// #Regression #Conformance #TypesAndModules #Unions 
#light

// Verify that null is not a proper value for a Discriminated Union


type DU = A of string | B of int | C

let x : DU = null

exit 1
