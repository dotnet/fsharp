// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Naked generics are not allowed in anonymous union types

type StringOr<'a> = ('a|string)  // Error: type variable in union