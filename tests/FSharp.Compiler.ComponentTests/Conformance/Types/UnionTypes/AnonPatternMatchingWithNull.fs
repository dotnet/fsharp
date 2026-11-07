// Anonymous Union Types
// Pattern matching on anonymous union types with null as a constituent case

let decide (x: (int|string|null)) =
    match x with
    | :? int as y -> 1
    | :? string as y -> 2
    | null -> 3

if not (decide 42 = 1) then failwith "Test failed"
if not (decide "asd" = 2) then failwith "Test failed"
if not (decide null = 3) then failwith "Test failed"
