// #Regression #Conformance #DeclarationElements #Import 

//<Expects status="error" span="(20,9-20,10)" id="FS0039">The value or constructor 'A' is not defined</Expects>
//<Expects status="error" span="(27,9-27,10)" id="FS0039">The value or constructor 'B' is not defined</Expects>
//<Expects status="error" span="(34,9-34,10)" id="FS0039">The value or constructor 'C' is not defined</Expects>


module TestModule1 =
    
    type Enum =
         | A = 0
         | B = 1
         | C = 2

// DU tags should now be in scope
open TestModule1

let mutable successes = 0

let a = A
(*let _ = 
    match a with 
    | A _ -> successes <- successes + 1
             ()
    | _ -> failwith "error"*)

let b = B
(*let _ = 
    match b with
    | B -> successes <- successes + 1 
           ()
    | _ -> failwith "error"*)

let c = C
(*let _ =
    match c with
    | C -> successes <- successes + 1
           ()
    | _ -> failwith "error"*)

// Everything worked out ok
if successes = 3 then
    ()
else
    failwith "Failed: 1"
