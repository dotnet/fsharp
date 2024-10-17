// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
// This means it cannot start with a string

#light

type T = | (* *) A  // ok
         | "A"      // err
