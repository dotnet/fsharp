// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter





type a = | ı of int     // err: Case labels for union types must be uppercase identifiers
         | i of int     // err: Case labels for union types must be uppercase identifiers
