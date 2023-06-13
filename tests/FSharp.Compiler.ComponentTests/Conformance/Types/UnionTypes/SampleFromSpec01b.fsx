// #Conformance #TypesAndModules #Unions 
// Union Types
// Sample from spec - using #light "off"
//<Expects status="success"></Expects>
#light "off"

(* extra space *) type Message = 
| Result of string
  | Request of int * string
    with
member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
    end  
    
let p = Result("Result");;
let q = Request(0,"Request");;

if p.Name = "Result" && q.Name = "Request" then 0 else failwith "Failed: 1"
