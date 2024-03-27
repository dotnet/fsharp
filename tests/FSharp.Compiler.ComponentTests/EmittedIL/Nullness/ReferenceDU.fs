module MyTestModule

[<NoComparison;NoEquality>]
type MyDu = 
    | JustLabel
    | JustInt of int
    | MaybeString of (string | null)

let giveMeLabel () = JustLabel

let createMaybeString (innerValue) = MaybeString innerValue

let processNullableDu (x : (MyDu | null)) : string | null =
    match x with
    | null -> null
    | JustLabel -> null
    | JustInt x -> string x
    | MaybeString x -> x