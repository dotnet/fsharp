// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:1165
// F# supports custom attributes on return type.
// Multiple attributes on the same return value (same attribute repeated twice) not allowed by default (AllowMultiple is false)
// Note: the syntax is kind of awkward...
//<Expects id="FS0429" span="(16,28-16,31)" status="error">The attribute type 'CA1' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>

type CA1 = 
    class
        inherit System.Attribute
        new (tt:string) = { t = tt }
        val t : string
    end                       

// This is a function that has a custom attribute on the return type.
let f(x) : [<CA1("A1")>] [<CA1("A2")>] int = x + 1
