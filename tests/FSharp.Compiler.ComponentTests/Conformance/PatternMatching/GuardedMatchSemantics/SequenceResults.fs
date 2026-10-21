module Test

let choose (xs: int array) =
    {{OPEN}}
        let (|Positive|_|) n = n > 0
        for n in xs do
            {{MATCH_PREFIX}}match n with
            {{CASE_INDENT}}| 0 | 1 -> {{EMPTY}}
            {{CASE_INDENT}}| _ when n < 0 -> {{EMPTY}}
            {{CASE_INDENT}}| 2 | 3 | Positive as value -> {{SUCCESS}}
            {{CASE_INDENT}}| _ -> {{EMPTY}}
    {{CLOSE}}

[<EntryPoint>]
let main _ =
    let values = [|-1; 0; 1; 2; 3; 4|]
    if (choose values |> Seq.toArray) <> [|2; 3; 4|] then failwith "sequence result"
{{ALLOCATION_CHECK}}
    0
