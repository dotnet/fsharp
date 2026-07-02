// Anonymous Union Types
// Anonymous union types are commutative

let id (x: (int | string)): (int | string) = x

let y: (string | int) =  "hello"
id y |> ignore