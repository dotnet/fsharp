// #Regression 
let failures = ref false
let report_failure () = 
  System.Console.Error.WriteLine "NO"; failures := true

let struct (_x,_y) = struct (1,2)

let _ = 
  if !failures then (System.Console.Out.WriteLine "Test Failed"; exit 1) 
  else
      (System.Console.Out.WriteLine "Test Passed"; 
       System.IO.File.WriteAllText("test.ok", "ok"); 
       exit 0)

