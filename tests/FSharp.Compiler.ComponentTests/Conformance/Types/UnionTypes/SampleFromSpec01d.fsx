// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// Sample from spec - using #light "off"
// The with/end tokens can be omitted when using the #light syntax option as long as the 
// type-defn-elements vertically aligns with the first ‘|’ in the  union-cases
// Regression test for FSHARP1.0:3707
//<Expects status="success"></Expects>
#light

(* extra space *) type Message = 
                  | Result of string
                  | Request of int * string
                   member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm

type MessagX = 
| Result of string
| Request of int * string
 member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
     
let p = Result("Result");;
let q = Request(0,"Request");;

if p.Name = "Result" && q.Name = "Request" then 0 else failwith "Failed: 1"
