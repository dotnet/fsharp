module Test

let run input =
    match input with
    | None, (100 | 101) when false -> -1
{{CLAUSES}}
    | Some value, _ when value = 0 -> value
    | Some _, _ -> 99

[<EntryPoint>]
let main _ =
    if run (Some 0, 200) <> 0 || run (Some 42, 0) <> 99 then failwith "join result"
    try
        run (None, 200) |> ignore
        failwith "missing match failure"
    with :? MatchFailureException -> ()
    0
