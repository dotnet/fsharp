// #Conformance #TypesAndModules #Exceptions 
// Make sure we properly detect bogus named field in constructors
//<Expects id="FS3174" span="(8,17-8,19)" status="error">The exception 'AAA' does not have a field named 'V3'\.</Expects>
//<Expects id="FS3174" span="(13,7-13,9)" status="error">The exception 'AAA' does not have a field named 'V3'\.</Expects>

exception AAA of V1 : int * V2 : string

raise <| AAA(1, V3 = "")

try
    raise <| AAA(1, "")
with
| AAA(V3 = "") -> ()
| _ -> ()