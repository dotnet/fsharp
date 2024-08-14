// #Conformance #TypesAndModules #Exceptions 
// Make sure we properly detect field names specified multiple times
//<Expects id="FS3175" span="(8,22-8,24)" status="error">Union case/exception field 'V1' cannot be used more than once\.</Expects>
//<Expects id="FS3175" span="(13,16-13,18)" status="error">Union case/exception field 'V2' cannot be used more than once\.</Expects>

exception AAA of V1 : int * V2 : string

raise <| AAA(V1 = 1, V1 = "")

try
    raise <| AAA(1, "")
with
| AAA(V2 = ""; V2 = "") -> ()
| _ -> ()