// Anonymous Union Types
// Pattern matching on anonymous union types - two columns, with null

let decide (x: (int|string|null), y: (int|string|null)): int =
    match (x, y) with
        | :? string, _ -> 1
        | :? int, _ -> 2
        | null, :? int -> 3
        | null, :? string -> 4
        | null, null -> 5

if not (decide ("asd", 1) = 1) then failwith "Test failed"
if not (decide (42, 42) = 2) then failwith "Test failed"
if not (decide (null, 1) = 3) then failwith "Test failed"
if not (decide (null, "asd") = 4) then failwith "Test failed"
if not (decide (null, null) = 5) then failwith "Test failed"
