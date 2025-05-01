// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 1622, Cannot use "null" in Attributes

// Test ability to use null in attributes

module m =
    [<Literal>]
    let Foo = (null : string)

[<System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)>]
type SomeAttribute(string:string) =
    inherit System.Attribute()

[<SomeAttribute("foo")>]        // Legal string
[<SomeAttribute(m.Foo)>]        // Null literal
[<SomeAttribute(null)>]         // Null
type Bar() =
    override this.ToString() = "Bar"


