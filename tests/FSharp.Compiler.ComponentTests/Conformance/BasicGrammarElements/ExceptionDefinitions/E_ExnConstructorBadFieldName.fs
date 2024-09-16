// #Conformance #TypesAndModules #Exceptions 
// Make sure we properly detect bogus named field in constructors



exception AAA of V1 : int * V2 : string

raise <| AAA(1, V3 = "")

try
    raise <| AAA(1, "")
with
| AAA(V3 = "") -> ()
| _ -> ()