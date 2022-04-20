// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSHARP1.0:6098
// conditionalattribute on a class should fail with a diagnostic
//<Expects id="FS1213" status="error" span="(10,6-10,9)">Attribute 'System.Diagnostics.ConditionalAttribute' is only valid on methods or attribute classes</Expects>
module M

open System.Diagnostics

[<Conditional("Debug")>]
type Foo() =
    
    [<Conditional("Debug")>]
    member x.Gar() = ()
