// #Regression #Conformance #SyntacticSugar 
#light

// Verify error if GetSlice is not defined
//<Expects id="FS0039" status="error">The type 'DU' does not define the field, constructor or member 'GetSlice'</Expects>

type DU = A | B of int | C

let t = B(42)
let _ = t.[*, ..4]

exit 1
