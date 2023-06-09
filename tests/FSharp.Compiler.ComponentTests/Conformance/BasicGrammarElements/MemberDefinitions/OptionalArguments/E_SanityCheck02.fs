// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
// Regression for FSHARP1.0: 6040
//<Expects id="FS1212" status="error">Optional arguments must come at the end of the argument list, after any non-optional arguments</Expects>
module M

type Foo =
    static member Bar(?x, y) = ()
Foo.Bar(2)
