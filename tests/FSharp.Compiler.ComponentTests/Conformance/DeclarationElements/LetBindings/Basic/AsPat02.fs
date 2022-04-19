// #Conformance #DeclarationElements #LetBindings 
#light

// Verify using patter matching inside let bindings

type DuType =
    | TagA of int * string
    | TagB


let ( TagA(x, y) ) = TagA(2, "foo")
if x <> 2     then failwith "Failed: 1"
if y <> "foo" then failwith "Failed: 2"

let _ =
    try
        let ( TagA(x, y) ) = TagB
        failwith "Failed: 3"
    with
    | :? MatchFailureException -> ()
    | _ -> failwith "Failed: 4"
