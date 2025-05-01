type Stuff =
    | A
    | B
    | C
let x v = 
    match v with
    | A -> ""
    | A as foo as foo1 -> ""
    | _ -> ""