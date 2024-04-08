// #Regression #Conformance #TypesAndModules 
// Type abbreviation - flexible type in signature
// Regression test for FSHARP1.0:3742
//<Expects id="FS0715" span="(15,16-15,18)" status="error">Anonymous type variables are not permitted in this declaration</Expects>
module E_FlexibleTypeInSignature01

type C = class
         end

[<Sealed>]
type D = class
            inherit C
         end
         
type BadType = #C           // <- flexible type (aka anonymous type with constraint)
