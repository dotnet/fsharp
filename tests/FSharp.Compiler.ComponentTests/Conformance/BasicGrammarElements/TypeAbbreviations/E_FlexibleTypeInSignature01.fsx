// #Regression #Conformance #TypesAndModules 
// Type abbreviation - flexible type in signature
// Regression test for FSHARP1.0:3742
//<Expects id="FS0191" status="error">AAA</Expects>
module E_FlexibleTypeInSignature01

type C = class
         end

[<Sealed>]
type D = class
            inherit C
         end

type BadType = D        // <-- no # (compare with corresponding .fsi)

exit 0
