// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Sample from spec - using #light (but with incorrect indentation)
// Q: Why this warning is emitted twice?
//<Expects id="FS0058" span="(9,1-9,2)" status="warning">Possible incorrect indentation: this token is offside of context started at position \(8:19\)\. Try indenting this token further or using standard formatting conventions</Expects>
#light

(* extra space *) type Message = 
| Result of string
  | Request of int * string
    with
member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
    end  
    
let p = Result("Result");;
let q = Request(0,"Request");;

if p.Name = "Result" && q.Name = "Request" then 0 else failwith "Failed: 1"



