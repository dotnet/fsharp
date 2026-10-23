// Anonymous Union Types
// Non-exhaustive pattern matching on anonymous union types

let decide (x: (int8|string|null)) =
    match x with
    | :? int8 as y -> 1
    | :? string as y -> 2