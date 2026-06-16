// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Pattern matching on anonymous union types with subtype inclusion
//<Expects status="success"></Expects>

let prettyPrint2 (x: (int8|int16|int64|string)) =
    match x with
    | :? System.ValueType as y -> printfn "value type"
    | :? string as y -> printfn "string"

prettyPrint2 42y