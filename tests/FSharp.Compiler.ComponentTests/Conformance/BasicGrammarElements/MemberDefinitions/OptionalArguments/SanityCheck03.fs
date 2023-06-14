// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
// Regression for FSHARP1.0: 6040
module M

type Foo =
    static member Bar(x, ?y) = ()
    static member Bar2(?y) = ()
Foo.Bar(2)
Foo.Bar2()
