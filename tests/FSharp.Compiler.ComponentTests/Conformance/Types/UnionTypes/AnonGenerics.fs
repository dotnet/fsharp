// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Basic syntax of anonymous union types
//<Expects status="success"></Expects>

type x<'T> = (list<'T> | int)

let a: x<int> = 1