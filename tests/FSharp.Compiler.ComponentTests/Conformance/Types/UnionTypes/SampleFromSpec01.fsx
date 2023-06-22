// #Conformance #TypesAndModules #Unions 
// Union Types
// Sample from spec
#light

type Message = 
    | Result of string
    | Request of int * string
    member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
    
let p = Result("Result")
let q = Request(0,"Request")

if p.Name = "Result" && q.Name = "Request" then 0 else failwith "Failed: 1"
