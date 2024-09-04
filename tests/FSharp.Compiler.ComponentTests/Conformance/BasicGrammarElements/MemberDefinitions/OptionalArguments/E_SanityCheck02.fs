// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
// Regression for FSHARP1.0: 6040

module M

type Foo =
    static member Bar(?x, y) = ()
Foo.Bar(2)
