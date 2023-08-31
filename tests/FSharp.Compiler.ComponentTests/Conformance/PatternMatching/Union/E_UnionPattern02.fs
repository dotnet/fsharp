type Stuff =
    | A of string * int
    | B of string * int
    | C
let x v = 
  match v with
  | A(a, b) -> ()
  | B(a, b) -> ()
  | A(a, b) | B(a, b) as f -> ()