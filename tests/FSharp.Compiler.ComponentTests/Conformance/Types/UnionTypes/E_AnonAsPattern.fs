// Anonymous Union Types
// Anonymous union cannot be used as a pattern type in a match expression

let decide (x: (int8|int16|int64|string)) =
    match x with
    | :? (int8|string) -> 0
    | _ -> 1

decide 42y |> ignore