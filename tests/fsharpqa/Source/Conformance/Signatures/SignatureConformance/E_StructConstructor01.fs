// #Conformance #SignatureFiles #Structs #Regression
// Regression for Dev11:137930, structs used to not give errors on unimplemented constructors in the signature file

module M

[<Struct>]
type Foo<'T> =
    val offset : int
    new (x:'T) = { offset = 1 } 
 
let foo = Foo<int>()