#r "globalNamespaceTP.dll"

let mutable failures = []
let reportFailure s = 
  stdout.WriteLine "\n................TEST FAILED...............\n"; failures <- failures @ [s]

let argv = System.Environment.GetCommandLineArgs() 
let SetCulture() = 
  if argv.Length > 2 && argv.[1] = "--culture" then  begin
    let cultureString = argv.[2] in 
    let culture = new System.Globalization.CultureInfo(cultureString) in 
    stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
    System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  end 
  
do SetCulture()    

let check s e r = 
  if r = e then  stdout.WriteLine (s+": YES") 
  else (stdout.WriteLine ("\n***** "+s+": FAIL\n"); reportFailure s)

let test s b = 
  if b then ( (* stdout.WriteLine ("passed: " + s) *) ) 
  else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)


printfn "%A" System.DateTime.Now

(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

let _ = 
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)


