// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Anonymous union types are associative
//<Expects status="success"></Expects>

let id (x: ((int | string) | float)): (int | string | float) = x

let y: (int | (string | float)) =  "hello"
id y |> ignore