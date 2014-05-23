//
//Test #r with shadowcopy disabled
//
open System;;
open System.Diagnostics;;

let compiled wait =
    let c = Process.Start(Environment.GetEnvironmentVariable("FSC"), "Library1.fs --target:library")
    c.WaitForExit(wait) |> ignore
    if c.ExitCode = 0 then true else false;;

//  Build the library
let first = compiled 10000;;
    
//reference it will lock the assembly because FSI was started with --shadowcopyreferences-
#r "Library1.dll";;

let next = compiled 10000;;
  
//compile will fail because shadow copy is disabled
if next = false then
    printfn "Succeeded -- compile fail because file locked due to --shadowcopyreferences-"
    use os = System.IO.File.CreateText "test1.ok" 
    os.Close();;
#quit;; 
