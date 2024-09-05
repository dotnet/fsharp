// #Conformance #TypesAndModules #Exception
// Make sure we properly detect when field names collide with member names




exception AAA of int * string * V3 : float
    with
    member this.Data0 = ""
    member this.Data1 = 3
    member this.V3 = 'x'