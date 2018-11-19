// #NoMT #CompilerOptions #RequiresENU   
open System
open System.IO


let fn1 = fsi.CommandLineArgs.[1]
let fn2 = fsi.CommandLineArgs.[2]

let f1 = File.ReadAllLines fn1 |> Array.toList
let f2 = File.ReadAllLines fn2 |> Array.toList

let mutable i = 0
let compare (f1:string list) (f2:string list) = 
    (f1,f2) ||> List.forall2 (fun (a:string) (b:string) ->
        let aa = System.Text.RegularExpressions.Regex.Replace(a, @"F# Compiler version .+", "F# Compiler")
        let bb = System.Text.RegularExpressions.Regex.Replace(b, @"F# Compiler version .+", "F# Compiler")

        i <- i+1
        if (aa = bb) then
            true 
        else
            printfn "Files differ at line %d:" i
            printfn "\t>> %s" a
            printfn "\t<< %s" b
            false
    ) 

let update = try Environment.GetEnvironmentVariable("TEST_UPDATE_BSL") = "1" with _ -> false

if update then 
    printfn "Updating %s --> %s" fn1 fn2
    File.Copy(fn1, fn2, true)

let exitCode = 
    if f1.Length = f2.Length then 
         if compare f1 f2 then 0 
         else printfn "File contents differ"; 1 
    else printfn "File lengths differ"; 1

exit exitCode
