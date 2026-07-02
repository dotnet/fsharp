// Anonymous Union Types
// Pattern matching on anonymous union types

let decide (x: (int8|int16|int64|string)): int =
    match x with
    | :? int8 -> 1
    | :? int16 -> 2
    | :? int64 -> 3
    | :? string -> 4

if not (decide 42y = 1) then failwith "Test failed"
if not (decide "hello" = 4) then failwith "Test failed"