
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Sample_MonoDevelop_3_2_8_Console.Main

open System

let someFunction x y = x + y

[<EntryPoint>]
let main args = 
    Console.WriteLine("Hello world!")
    0

