module Test

let rec recurse tag items =
    match tag, items with
    | 0, [] -> 0
    | _, _ :: rest when recurse tag rest >= 0 -> 1 + recurse tag rest
    | (1 | 2), _ when true -> -1
    | _ -> 0

let check token =
    try
#nowarn "21" "22" "40"
        let rec first =
            match token with
            | 0 when {{USE}} = 42 -> 1
            | _ when {{USE}} = 42 -> 2
            | (1 | 2) when false -> 3
            | _ -> 4
        and later = 42
#warnon "21" "22" "40"
        first
    with :? System.InvalidOperationException -> -1

[<EntryPoint>]
let main _ =
    if (recurse 0 [1; 2], recurse 0 ["x"]) <> (2, 1) then failwith "recursive instantiation"
    for token in 0 .. 3 do
        if check token <> {{EXPECTED}} then failwith "recursive initialization"
    0
