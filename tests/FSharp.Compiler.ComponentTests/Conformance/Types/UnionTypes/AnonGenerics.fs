// Anonymous Union Types
// Basic syntax of anonymous union types

type x<'T> = (list<'T> | int)

let a: x<int> = 1