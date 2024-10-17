// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6098
// conditionalattribute on a class should fail with a diagnostic

module M

open System.Diagnostics

[<Conditional("Debug")>]
type Foo() =
    
    [<Conditional("Debug")>]
    member x.Gar() = ()
