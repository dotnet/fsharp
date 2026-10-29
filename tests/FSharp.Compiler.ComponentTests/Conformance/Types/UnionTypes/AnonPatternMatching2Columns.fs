// Anonymous Union Types
// Pattern matching on anonymous union types - two columns

let decide (x: (int|bool), y: (int|bool)): int =
    match (x, y) with
        | :? bool, _ -> 1
        | _, :? int  -> 2
        | _, :? bool -> 3

if not (decide (true, false) = 1) then failwith "Test failed"
if not (decide (42, 42) = 2) then failwith "Test failed"
if not (decide (42, true) = 3) then failwith "Test failed"