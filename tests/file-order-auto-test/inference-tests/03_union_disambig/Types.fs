module UnionTest.Types

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

type Command =
    | Start
    | Stop
    | Reset

type Expr =
    | Const of int
    | Add of Expr * Expr
    | Mul of Expr * Expr
