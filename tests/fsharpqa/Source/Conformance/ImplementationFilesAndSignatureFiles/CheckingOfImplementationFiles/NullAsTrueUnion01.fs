// #Regression #Conformance #SignatureFiles 
// Regression for FSHARP1.0:6094
// nullary union cases and signature files

module Foo

open System

// used to warn
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = 
 | A
 | B of string
 with
   override x.ToString() = "x"

// always worked
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU2 = 
 | A
 | B of string

let x = A // make sure it's actually represented with null
exit <| 
    try 
        x.ToString() |> ignore
        -1 
    with 
        | :? NullReferenceException -> 0 
        | _ -> 1
