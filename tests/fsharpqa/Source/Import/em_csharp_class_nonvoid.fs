// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:1494
// Make sure F# can import C# extension methods
module M

type T() = class 
            inherit C()
           end

let t = T()

t.M4(1.0M, 0.3f)       |> ignore
