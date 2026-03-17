// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:2439
// fsi.CommandLineArgs
// scenario: one argument
let x = Seq.length fsi.CommandLineArgs
let y = fsi.CommandLineArgs.[1]
printfn "%A %A" x y
if (x <> 2) || (y <> "Hello") then exit(1)
exit(0);;
