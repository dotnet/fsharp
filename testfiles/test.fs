type System.Int32 with
    static member Add(a : System.Int32, b : System.Int32) = a + b

type MyType =
    | MyType of int
    static member Add(MyType x, MyType y) = MyType (x + y)

let inline addGeneric< ^A when ^A : (static member Add : ^A * ^A -> ^A) > (a,b) : ^A =
    (^A : (static member Add : ^A * ^A -> ^A) (a,b))

let inline (+++) a b = addGeneric(a,b)

[<EntryPoint>]
let main args =
    MyType(1) +++ MyType(2) |> ignore
    1 +++ 1