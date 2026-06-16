// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Possible overlap of constituent types

type x<'T, 'U> = (list<'T> | list<'U>)