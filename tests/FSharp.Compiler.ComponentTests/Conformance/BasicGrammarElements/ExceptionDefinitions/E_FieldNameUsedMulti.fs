// #Conformance #TypesAndModules #Exceptions 
// Make sure we properly detect field names specified multiple times



exception AAA of V1 : int * V2 : string

raise <| AAA(V1 = 1, V1 = "")

try
    raise <| AAA(1, "")
with
| AAA(V2 = ""; V2 = "") -> ()
| _ -> ()