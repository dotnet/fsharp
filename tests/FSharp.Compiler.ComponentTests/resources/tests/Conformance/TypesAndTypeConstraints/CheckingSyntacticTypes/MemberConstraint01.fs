// #Conformance #TypeConstraints #Diagnostics

let inline testFunc (a : ^x) =
    (^x : (static member someFunc : unit -> ^x) ())

type Foo(x) =
    member this.Test = x
    static member someFunc() = Foo(3)

let f = testFunc (Foo(1))
let r = f.Test

exit <| (if r = 3 then 0 else 1)