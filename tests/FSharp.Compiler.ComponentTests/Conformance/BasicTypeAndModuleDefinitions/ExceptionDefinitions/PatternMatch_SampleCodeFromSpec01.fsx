// #Conformance #TypesAndModules #Exceptions 
// This is the sample code that appears in the specs under 9.4
// It shows how to use exceptions in pattern match
//<Expects status="success"></Expects>
#light

exception Error of int * string

try 
    raise (Error(3, "well that didn't work did it"))
with 
    | Error(sev, msg) -> printfn "severity = %d, message = %s" sev msg
