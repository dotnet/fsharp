// #Conformance #TypesAndModules #Unions
// Make sure we properly detect when field names collide with member names
// Note: this only applies to single-case DUs





type MyDU =
    | Case1 of int * string * V3 : float
    with
    member this.Item1 = ""
    member this.Item2 = 3
    member this.V3 = 'x'

type MyDU2 = 
    | Case1 of int
    with
    member this.Item = ""