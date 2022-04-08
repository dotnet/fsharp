// #NoMono #NoMT #CodeGen #EmittedIL   
#light

type U = U of int * int

let TestFunction21(U(a,b)) =
    printfn "a = %A, a = %A" a b
