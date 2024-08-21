// #Conformance #DeclarationElements #Attributes 
#light

// Verify the ability to specify types as parameters into Attributes

type CustomAttribute(x : System.Type) =
    inherit System.Attribute()
    

[<CustomAttribute(typeof<System.String>)>]
type Foo() =
    member this.Value = 1

