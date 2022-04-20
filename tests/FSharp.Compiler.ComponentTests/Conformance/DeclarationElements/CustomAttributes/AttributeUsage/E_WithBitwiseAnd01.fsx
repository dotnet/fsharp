// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:4035
// Using bitwise AND (&&&) in AttributeUsage should give a reasonable error
//<Expects id="FS0191" status="error">Review when fixed</Expects>
#light

[<System.AttributeUsage(System.AttributeTargets.Class &&& System.AttributeTargets.Assembly, AllowMultiple=true)>]  
[<Sealed>]
type FooAttribute() =
    inherit System.Attribute()

[<Foo>]
let x = 1
