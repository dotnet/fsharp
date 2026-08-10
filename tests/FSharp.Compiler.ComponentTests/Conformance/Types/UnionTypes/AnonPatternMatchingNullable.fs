// Anonymous Union Types
// Pattern matching on anonymous union types with null as a constituent case

let decide (x: (int8|int16|null)) =
    match x with
    | :? int16 as y -> 1
    | :? System.ValueType as y -> 2
    | null -> 3

if not (decide 42s = 1) then failwith "Test failed"
if not (decide 42y = 2) then failwith "Test failed"
if not (decide null = 3) then failwith "Test failed"
