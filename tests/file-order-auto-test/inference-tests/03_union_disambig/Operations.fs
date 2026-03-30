module UnionTest.Operations

open UnionTest.Types

let area (s: Shape) =
    match s with
    | Circle r -> System.Math.PI * r * r
    | Rectangle(w, h) -> w * h

let rec eval (e: Expr) =
    match e with
    | Const n -> n
    | Add(a, b) -> eval a + eval b
    | Mul(a, b) -> eval a * eval b

let describe (cmd: Command) =
    match cmd with
    | Start -> "starting"
    | Stop -> "stopping"
    | Reset -> "resetting"
