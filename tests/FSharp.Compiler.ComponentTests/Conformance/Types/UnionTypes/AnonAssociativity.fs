// Anonymous Union Types
// Anonymous union types are associative

type IntString = (int | string)
type StringFloat = (string | float)

let id (x: (IntString | float)): (int | string | float) = x

let y: (int | StringFloat) =  "hello"
id y |> ignore