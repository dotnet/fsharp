// #Regression #Conformance #SyntacticSugar 
#light

// Verify error if GetSlice is not defined
//<Expects id="FS0039" status="error">The field, constructor or member 'GetSlice' is not defined</Expects>

type DU = A | B of int | C

let t = B(42)
let _ = t.[*, ..4]

exit 1
