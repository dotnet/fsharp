// #Conformance #DeclarationElements #LetBindings 
#light

// Verify using patter matching inside let bindings

type DuType =
    | TagA of int * string
    | TagB


let ( TagA(x, y) ) = TagA(2, "foo")
if x <> 2     then exit 1
if y <> "foo" then exit 1

let _ =
    try
        let ( TagA(x, y) ) = TagB
        exit 1
    with
    | :? MatchFailureException -> ()
    | _ -> exit 1


exit 0
