// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
// This means it cannot start with a digit

#light

type a = | 1 of int         // err: can't use 
