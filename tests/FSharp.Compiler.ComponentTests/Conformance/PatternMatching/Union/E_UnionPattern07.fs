type Stuff =
    | A
    | B
    | C
let x v z = 
    match v with
    | A -> ""
    | A as foo when z > 5 -> ""
    | C -> ""