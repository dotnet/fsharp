// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Implicit conversion is added for expressions where, if the known type information for of an expression is "must convert to" an anonymous union type,
// and the type of the expression is a nominal type prior to its commitment point, then that type must convert to one of the constituent types of the anonymous union type.
//<Expects status="success"></Expects>

let intOrString : (int|string) = if true then 1 else "Hello"