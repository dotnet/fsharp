// #Conformance #TypesAndModules #Unions 
// Should be able to pattern match using named or positional fields
//<Expects status="success"></Expects>
type MyDU = 
    | Case1 of int * int * int * Named1 : int * Named2 : int * int

let o = Case1(1, 1, 1, 7, 5, 1)
let m =
    match o with
    | Case1(0, _, _, _, _, _) -> 1
    | Case1(Named1 = 2) -> 1
    | Case1(1, 1, 1, 0, 1, 1) -> 1
    | Case1(Named2 = 5; Named1 = 7) -> 0
    | _ -> 1

let (Case1(Named1 = x)) = o
if x = 7 then ()
else failwith "Failed: 1"

if m <> 0 then failwith $"Failed: {m}"
