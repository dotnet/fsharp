// Anonymous Union Types
// Pattern matching on anonymous union types with subtype inclusion

let decide (x: (int8|int16|int64)): int =
    match x with
    | :? int8 as y -> 0
    | :? System.ValueType as y -> 1

if not (decide 42y = 0) then failwith "Test failed"