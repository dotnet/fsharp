// #Regression #Conformance #TypesAndModules #Records 
// Verify appropriate error with ambiguous record inference
// Regression test for FSHARP1.0:2780
//<Expects id="FS0764" span="(12,15-12,24)" status="error">No assignment given for field 'Y' of type 'N\.Blue'</Expects>

module N

type Red  = { A : int }
type Blue = { X : int; Y : int }

let aBlue   = { X = 0; Y = 1 }
let unknown = { X = 0 }
