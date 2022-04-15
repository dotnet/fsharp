// #Conformance #TypesAndModules #Exceptions 
// Specify and match on named fields
//<Expects status="success"></Expects>

exception AAA of V1 : int * V2 : int * V3 : string
exception BBB of bool * bool

let result =
    try
        raise <| AAA(5, V2 = 10, V3 = "")
    with
    | AAA(1, 1, "") -> 1
    | BBB(_, false) -> 1
    | AAA(V1 = 10) -> 1
    | AAA(V3 = ""; V2 = 10; V1 = 5) -> 0
    | _ -> 1

if result <> 0 then failwith "Failed: 1"

let x = AAA(5, V2 = 10, V3 = "")
let (AAA(V3 = y)) = x
if y <> "" then failwith "Failed: 2"

exception CCC of bField : bool * iField : int

let test = 10
let xx = CCC(test = 10, iField = 1)

// Data0 should not conflict with V1
exception DDD of V1 : int
    with
    member this.Data0 = ""

// Message field name should not conflict with base System.Exeption.Message member
exception EEE of Message : int
