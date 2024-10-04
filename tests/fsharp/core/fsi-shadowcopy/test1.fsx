//
//Test #r with shadowcopy disabled
//
open System;;
open System.Diagnostics;;

let compiled wait =
    let psi = ProcessStartInfo(Environment.GetEnvironmentVariable("FSC"), "Library1.fs --target:library")
    psi.CreateNoWindow <- true
    psi.UseShellExecute <- false
    let c = Process.Start(psi)
    c.WaitForExit(wait) |> ignore
    if c.ExitCode = 0 then true else false;;

//  Build the library
let first = compiled 30000;;
    
//reference it will lock the assembly because FSI was started with --shadowcopyreferences-
#r "Library1.dll";;

let next = compiled 30000;;
  
//compile will fail because shadow copy is disabled
if next = false then
    printfn "Succeeded -- compile fail because file locked due to --shadowcopyreferences-"
    printf "TEST PASSED OK" ;
else
    printfn "Failed -- compile succeeded but should have failed due to file lock because of --shadowcopyReferences-.  Suspect test error";;

#quit;; 
