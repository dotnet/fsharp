// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:3797
// Attributes defined in F# and whose AttributeUsage is specified using bitwise or do MUST have the attribute usage checked by the F# compiler
//<Expects id="FS0842" span="(12,3-12,6)" status="error">This attribute is not valid for use on this language element</Expects>
#light

[<System.AttributeUsage(System.AttributeTargets.Class ||| System.AttributeTargets.Assembly, AllowMultiple=true)>]  
[<Sealed>]
type FooAttribute() =
    inherit System.Attribute()

[<Foo>]     // <-- before the fix for #3797, the compiler was NOT emitting an error here!
let v = 2
