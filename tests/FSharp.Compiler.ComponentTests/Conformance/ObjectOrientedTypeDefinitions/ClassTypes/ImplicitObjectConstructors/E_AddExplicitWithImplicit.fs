// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Verify compiler error when adding an explicit constructor to a type which
// already has an implicit constructor.

//<Expects id="FS0762" status="error" span="(8,13)">Constructors for the type 'Foo' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression</Expects>

type Foo(x : int) =
    new() = { }   // ERROR: Adding explicit ctor which doesn't call implicit ctor
    member this.Value = x

exit 1
