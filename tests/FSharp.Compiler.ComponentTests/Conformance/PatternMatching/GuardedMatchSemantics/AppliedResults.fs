module Test

type U = A | B

let choose (input: U * int) =
    (match input with
     | (A | B), 1 when false -> fun () -> 1
     | _ -> fun () -> 2) ()

let apply (f: int -> int) g gate token flag =
    (match token, flag with
     | (0 | 1), true when gate -> f
     | _, true -> g
     | _ -> f) 42

[<EntryPoint>]
let main _ =
    for tag in [A; B] do
        for n in [0; 1] do
            if choose (tag, n) <> 2 then failwith "applied unit result"
    for gate in [false; true] do
        for flag in [false; true] do
            for token in [0; 1; 2] do
                let expected = if not flag || gate && token <> 2 then 43 else 44
                if apply ((+) 1) ((+) 2) gate token flag <> expected then failwith "applied result"
    0
