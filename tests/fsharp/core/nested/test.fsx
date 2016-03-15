// #Conformance 
#if Portable
module Core_nested
#endif

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

#if NetCore
#else
let argv = System.Environment.GetCommandLineArgs() 
let SetCulture() = 
  if argv.Length > 2 && argv.[1] = "--culture" then  begin
    let cultureString = argv.[2] in 
    let culture = new System.Globalization.CultureInfo(cultureString) in 
    stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
    System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  end 
  
do SetCulture()    
#endif

let f () = 3

let wher = ref []
let spot x = wher := !wher @ [x]; stderr.WriteLine(x:string)
do spot "Initialized before X1 OK"

module X1 = begin 
  type x = X | Y
  let y = 3

  do spot "Initialized X1 OK";

end


module X2 = begin 
  type x = X | Y
  let x = 3
  let y () = X
  let z = x + (match y() with X -> 4 | Y -> 5)
  do spot "Initialized X2 OK";
end


module X3 = begin 
  let y = X2.X
  do spot "Initialized X3 OK";
end


do spot "Initialized after X3 OK"

let _ = X2.z + X2.x + X1.y 

do test "uyf78" (!wher = [ "Initialized before X1 OK";
                           "Initialized X1 OK";
                           "Initialized X2 OK";
                           "Initialized X3 OK";
                           "Initialized after X3 OK" ])

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 


do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)