// #Regression #Conformance #TypesAndModules #Unions 
#light

// Verify that null is not a proper value for a Discriminated Union
//<Expects id="FS0043" status="error">The type 'DU' does not have 'null' as a proper value</Expects>

type DU = A of string | B of int | C

let x : DU = null

exit 1
