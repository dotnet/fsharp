// #Regression #Conformance #TypeConstraints 
#light
// Regression test for FSHARP1.0:3880
// Type With Null As representation value
// It is not allowed to use null directly!

// Unit type
let u : unit = null

// Option type: None value
let o : int option = null

// Same as above, but in general for DU
[<Microsoft.FSharp.Core.CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = | NULL
          | Case of int

let du : DU = null

//<Expects id="FS0043" span="(8,16-8,20)" status="error">The type 'unit' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(11,22-11,26)" status="error">The type 'int option' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(18,15-18,19)" status="error">The type 'DU' does not have 'null' as a proper value</Expects>
