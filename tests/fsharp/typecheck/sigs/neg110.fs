module rec Neg110
// check that a decent error is given when a type that does not inherit from System.Attribute is used as attribute
type NotAttribute() = class end

[<NotAttribute>]
type T() = class end

// note this test shouldn't contain any other code
