// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Non-exhaustive pattern matching on anonymous union types

let prettyPrint (x: (int8|int16|int64|string)) =
    match x with
    | :? int8 as y -> printfn "int8: %d" y
    | :? int16 as y -> printfn "int16: %d" y
    | :? int64 as y -> printfn "int64: %d" y

prettyPrint 42y