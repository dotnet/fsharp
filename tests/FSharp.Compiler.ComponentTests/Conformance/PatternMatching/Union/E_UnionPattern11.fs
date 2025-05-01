type Stuff =
    | A of a: string * b: int
    | B
    | C
let x v = 
    match v with
    | A(s, i) -> ""
    | A(b=i) as foo as foo1 as foo3 as foo4 when i = 0 -> ""
    | _ -> ""