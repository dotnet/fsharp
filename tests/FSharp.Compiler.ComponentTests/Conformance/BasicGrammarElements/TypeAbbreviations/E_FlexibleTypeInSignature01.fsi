// #Regression #Conformance #TypesAndModules 
// Type abbreviation - flexible type in signature
// Regression test for FSHARP1.0:3742

module E_FlexibleTypeInSignature01

type C = class
         end

[<Sealed>]
type D = class
            inherit C
         end
         
type BadType = #C           // <- flexible type (aka anonymous type with constraint)
