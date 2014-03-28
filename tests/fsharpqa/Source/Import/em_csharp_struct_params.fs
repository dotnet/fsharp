// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:1494
// Make sure F# can import C# extension methods
module M

let s = S()

s.M3([|1.0M; -2.0M|])  |> ignore
