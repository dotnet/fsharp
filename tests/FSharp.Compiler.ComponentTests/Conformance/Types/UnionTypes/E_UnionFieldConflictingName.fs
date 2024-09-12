// #Conformance #TypesAndModules #Unions 
// Make sure we properly detect field naming collisions



type MyDU = 
    | Case1 of Item2 : int * string

type MyDU2 = 
    | Case1 of A : int * A : string * A : int