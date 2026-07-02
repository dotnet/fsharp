// Anonymous Union Types
// Anonymous union types are associative

let id (x: ((int | string) | float)): (int | string | float) = x

let y: (int | (string | float)) =  "hello"
id y |> ignore