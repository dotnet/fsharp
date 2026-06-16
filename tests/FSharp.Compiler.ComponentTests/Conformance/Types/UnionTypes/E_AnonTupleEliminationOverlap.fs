// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// One type is fully included in another w.r.t. runtime type tests (tuple elimination)

let y: ((int * int) | System.Tuple<int,int>) = (1,2)