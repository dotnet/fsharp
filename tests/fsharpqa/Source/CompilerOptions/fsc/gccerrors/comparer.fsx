open System
open System.IO
// #NoMT #CompilerOptions #RequiresENU   
let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]
// Read file into an array
let File2List(filename : string) = File.ReadAllLines filename |> Array.toList
let f1 = File2List fn1
let f2 = File2List fn2
let mutable i = 0

let compare f1 f2 = 
    (f1, f2) ||> List.forall2 (fun (a : string) (b : string) -> 
                     i <- i + 1
                     if (a = b) then true
                     else 
                         printfn "Files differ at line %d:" i
                         printfn "\t>> %s" a
                         printfn "\t<< %s" b
                         false)

let update = try Environment.GetEnvironmentVariable("TEST_UPDATE_BSL") = "1" with _ -> false

if update then 
    printfn "Updating %s --> %s" fn1 fn2
    File.Copy(fn1, fn2, true)

exit (if (f1.Length = f2.Length && compare f1 f2) then 0
      else 1)
