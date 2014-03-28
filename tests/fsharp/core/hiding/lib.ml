#light

type abstractType = { xx : int }

exception A
exception B of string
exception C = A

let x = { xx = 3 }

let e1 = A
let e2 = B("a")
let e3 = C

