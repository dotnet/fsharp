// #NoMono #NoMT #CodeGen #EmittedIL #Sequences #NETFX20Only #NETFX40Only 
module SeqExpressionSteppingTest7 // Regression test for FSHARP1.0:4454
// "Stepping into sequence expression pops up a dialog trying to located an unknown file"
let r = ref 0
let f () = [ if (incr r; true) then yield! failwith "" ]
printfn "res = %A" (try f () with Failure _ -> [!r])
