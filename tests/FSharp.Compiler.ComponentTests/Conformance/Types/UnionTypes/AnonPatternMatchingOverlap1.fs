// Anonymous Union Types
// Pattern matching on anonymous union types with subtype inclusion

let decide (x: (int8|int16|int64)): int =
    match x with
    | :? System.ValueType as y -> 0
    | :? int8 as y -> 1

if not (decide s42y = 0) then failwith "Test failed"