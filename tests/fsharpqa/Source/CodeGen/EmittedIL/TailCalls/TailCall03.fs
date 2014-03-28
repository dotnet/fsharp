// #NoMono #NoMT #CodeGen #EmittedIL #Tailcall
// Regression test for DevDiv:72571
let foo (x:int byref) (y:int byref) z = printfn "%d" (x+y)
let run() = let mutable x = 0 in foo &x &x 5
