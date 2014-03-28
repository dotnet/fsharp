// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Abbreviated type already used (redefined)
//<Expects id="FS0037" span="(7,6-7,7)" status="error">Duplicate definition of type, exception or module 'T'</Expects>
#light
type T = Microsoft.FSharp.Math.BigInt
type T = Microsoft.FSharp.Math.BigNum       // error!
