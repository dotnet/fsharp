//
//Test #r with shadowcopy enabled
//
open System;;
open System.Diagnostics;;

let compiled wait =
    let c = Process.Start(Environment.GetEnvironmentVariable("FSC"), "Library1.fs --target:library")
    c.WaitForExit(wait) |> ignore
    if c.ExitCode = 0 then true else false;;

//  Build the library
let first = compiled 10000;;
    
//reference it will not lock the assembly because FSI was started with --shadowCopyReferences+
#r "Library1.dll";;

let next = compiled 10000;;
  
//compile will succeed because shadow copy is enabled
if next = true then
    printfn "Succeeded -- compile worked because file not locked due to --shadowcopyReferences+"
    use os = System.IO.File.CreateText "test2.ok" 
    os.Close();;
#quit;; 
