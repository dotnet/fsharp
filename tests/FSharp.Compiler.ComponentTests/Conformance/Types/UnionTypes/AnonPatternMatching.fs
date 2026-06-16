// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Pattern matching on anonymous union types
//<Expects status="success"></Expects>

let prettyPrint (x: (int8|int16|int64|string)) =
    match x with
    | :? int8 as y -> printfn "int8: %d" y
    | :? int16 as y -> printfn "int16: %d" y
    | :? int64 as y -> printfn "int64: %d" y
    | :? string as y -> printfn "string: %s" y

prettyPrint 42y
prettyPrint "hello"