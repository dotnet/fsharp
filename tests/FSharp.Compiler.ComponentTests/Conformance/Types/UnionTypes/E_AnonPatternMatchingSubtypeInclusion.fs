// Anonymous Union Types
// Non-exhaustive pattern matching on anonymous union types with subtype inclusion

let prettyPrint2 (x: (int8|int16|int64|string)) =
    match x with
    | :? System.ValueType as y -> printfn "value type"

prettyPrint2 42y