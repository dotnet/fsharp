// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6098
// conditionalattribute on a class should fail with a diagnostic
// No error here because Foo inherits from Attribute

module M

open System.Diagnostics

[<Conditional("Debug")>]
type Foo() =
    inherit System.Attribute()
    
    [<Conditional("Debug")>]
    member x.Gar() = ()

