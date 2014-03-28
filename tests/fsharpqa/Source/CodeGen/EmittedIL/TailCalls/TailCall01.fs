// #NoMono #NoMT #CodeGen #EmittedIL #Tailcall
// Regression test for DevDiv:72571
let foo(x:int, y) = printfn "%d" x
let run() = let x = 0 in foo(x,5)
