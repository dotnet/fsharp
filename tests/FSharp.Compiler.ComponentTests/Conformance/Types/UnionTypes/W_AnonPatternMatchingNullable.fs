// Anonymous Union Types
// Non-exhaustive pattern matching on anonymous union types

let decide (x: (int8|int16|null)) =
    match x with
    | :? int16 as y -> 1
    | :? System.ValueType as y -> 2