// #Globalization #Regression
// Regression test for bug 25619
// F# doesn't encode Unicode surrogate characters correctly when obtained from a hex character literal
open System

let x = Char.ConvertFromUtf32(0x00020001)
let y = "\U00020001"
let x1 = x[0]
let x2 = x[1]
let y1 = y[0]
let y2 = y[1]
if (x1 <> y1) then 
    printfn "x1 == y1 => %b" (x1 = y1)
    raise (new Exception("exit 1"))

if (x2 <> y2) then 
    printfn "x2 == y2 => %b" (x2 = y2)
    raise (new Exception("exit 2"))

if  (int x2) <> (int y2) then
    printfn "x2 = %x, y2 = %x" (int x2) (int y2)
    raise (new Exception("exit 3"))

printfn "Finished"
