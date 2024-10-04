#r "provider.dll"

let mutable failures = []
let reportFailure s = 
  stdout.WriteLine "\n................TEST FAILED...............\n"; failures <- failures @ [s]

let check s e r = 
  if r = e then  stdout.WriteLine (s+": YES") 
  else (stdout.WriteLine ("\n***** "+s+": FAIL\n"); reportFailure s)

let test s b = 
  if b then ( (* stdout.WriteLine ("passed: " + s) *) ) 
  else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)


type T1 = FSharp.HelloWorld.HelloWorldType

let f (x: FSharp.HelloWorld.HelloWorldType) =  x

(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

let _ = 
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)


