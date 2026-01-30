// #Conformance #Signatures #Structs #Regression
// Regression for Dev11:137930, structs used to not give errors on unimplemented constructors in the signature file
//<Expects status="error" id="FS0193" span="(4,8-4,9)">Module 'M' requires a value 'new: unit -> Foo<'T>'</Expects>

module M

[<Struct>]
type Foo<'T> =
    new: unit -> 'T Foo