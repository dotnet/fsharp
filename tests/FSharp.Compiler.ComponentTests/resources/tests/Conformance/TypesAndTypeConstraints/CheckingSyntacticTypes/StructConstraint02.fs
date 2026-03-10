// #Conformance #TypeConstraints 
// Struct Unions should have the ValueType supertype constraint

[<Struct>]
type StructUnion = Case of int

let nullable = System.Nullable<StructUnion>()
