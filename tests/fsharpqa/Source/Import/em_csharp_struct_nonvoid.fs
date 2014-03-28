// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:1494
// Make sure F# can import C# extension methods
module M

let s = S()

s.M1(1.2M, 0.3f)       |> ignore


                                          
