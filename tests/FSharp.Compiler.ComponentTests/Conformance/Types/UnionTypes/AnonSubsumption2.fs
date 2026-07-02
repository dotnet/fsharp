// Anonymous Union Types
// (int | string | float) is a supertype of string

let id (x: (int | string | float)): (int | string | float) = x

let y: string =  "hello"
id y |> ignore