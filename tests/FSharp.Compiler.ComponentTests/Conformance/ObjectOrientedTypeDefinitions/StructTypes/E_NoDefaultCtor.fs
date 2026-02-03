// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Verify error when trying to create a default constructor for structs
//<Expects id="FS0081" status="error" span="(6,6-6,9)">Implicit object constructors for structs must take at least one argument$</Expects>
//<Expects id="FS0035" status="error" span="(7,5-7,38)">This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings$</Expects>
[<Struct>]
type Foo() =
    do printfn "Default constructor!"   (* *)