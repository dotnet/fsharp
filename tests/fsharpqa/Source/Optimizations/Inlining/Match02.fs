// #NoMono #CodeGen #Optimizations 
module Match02

[<Struct; NoEquality; NoComparison>]
type S = 
    static member inline (+)(_, _ : S) = 0
    static member inline (*)(_, _ : S) = 1
    static member inline (+)(_ : S, _) = 2
    static member inline (*)(_ : S, _) = 3

let testmethod () = 

    let a = (1,2) + S() // should be inlined - 0
    let b = (fun x -> x + 1) + S()// should be inlined 0
    let c = (3,4) * S() // should be inlined 1
    let d = (fun x -> x + 2) * S()// should be inlined 1
    let e = S() + (1.0, "") // should be inlined 2
    let f = S() + (fun () -> "1")// should be inlined 2
    let g = S() * (2.0, "") // should be inlined 3
    let h = S() * (fun () -> "2")// should be inlined 3

    a + b + c + d + e + f + g + h

