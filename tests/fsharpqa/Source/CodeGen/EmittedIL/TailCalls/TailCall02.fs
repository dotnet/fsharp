// #NoMono #NoMT #CodeGen #EmittedIL #Tailcall
// Regression test for DevDiv:72571
let foo(x:int byref) = x
let run() = let mutable x = 0 in foo(&x)

