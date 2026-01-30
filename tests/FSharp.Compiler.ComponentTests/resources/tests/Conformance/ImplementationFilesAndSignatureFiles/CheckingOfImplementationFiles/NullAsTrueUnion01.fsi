// #Regression #Conformance #SignatureFiles 
// Regression for FSHARP1.0:6094
// nullary union cases and signature files

module Foo

  [<CompilationRepresentationAttribute
    (enum<CompilationRepresentationFlags> (8))>]
  type DU =
    | A
    | B of string
    with
      override ToString : unit -> string
    end

  [<CompilationRepresentationAttribute
    (enum<CompilationRepresentationFlags> (8))>]
  type DU2 =
    | A
    | B of string
