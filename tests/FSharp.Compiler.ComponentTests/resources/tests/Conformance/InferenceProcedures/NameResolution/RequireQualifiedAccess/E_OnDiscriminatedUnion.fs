// #Regression #Conformance #TypeInference #Attributes 
// Verify an error if not fully-qualifying discriminated union
// when marked with RequireQualifiedAccess

//<Expects id="FS0039" status="error">The value or constructor 'B' is not defined</Expects>

[<RequireQualifiedAccess>]
type DiscUnion =
   | A
   | B of string
   | C of DiscUnion * int

let x = DiscUnion.C(B("oops"), 42)

exit 1
