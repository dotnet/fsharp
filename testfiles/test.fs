module Test

type System.Int32 with
    static member (++)(a: int, b: int) = a 

1 ++ 2


type System.Int32 with
    static member Add(a: int, b: int) = a 

type MyType =
    | MyType of int

[<AutoOpen>]
module Extensions = 
    type MyType with
        static member Add(MyType x, MyType y) = MyType (x + y)

let inline addGeneric< ^A when ^A : (static member Add : ^A * ^A -> ^A) > (a,b) : ^A =
    (^A : (static member Add : ^A * ^A -> ^A) (a,b))

let inline (+++) a b = addGeneric(a,b)

let inline addGeneric2  (a,b) : ^A when ^A : (static member Add : ^A * ^A -> ^A) =
    (^A : (static member Add : ^A * ^A -> ^A) (a,b))

let inline (++++) a b = addGeneric2(a,b)


let f () =
    let v1 = addGeneric (MyType(1), MyType(2))
    let v2 = addGeneric (1,1)
    ()

let f2 () =
    let v1 = MyType(1) +++ MyType(2)
    let v2 = 1 +++ 1
    1

let f3 () =
    let v1 = addGeneric2 (MyType(1), MyType(2))
    let v2 = addGeneric2 (1,1)
    ()

let f4 () =
    let v1 = MyType(1) ++++ MyType(2)
    let v2 = 1 ++++ 1
    ()  
