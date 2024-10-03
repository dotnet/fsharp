// #Regression 
let failures = ref false
let report_failure () = 
  System.Console.Error.WriteLine "NO"; failures := true



type typ = 
  | Type_void          
  | Type_other1 of int 
  | Type_other2 of int 


let _ = if (4, ".ctor",0, Type_void, []) =  (4, ".ctor",0, Type_void, [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success A"
let _ = if (4, ".ctor",0, Type_void, [Type_other2 4]) =  (4, ".ctor",0, Type_void, [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success B"
let _ = if (".ctor",0, Type_void, []) =  (".ctor",0, Type_void, [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success C"

let _ = if (0, Type_void, []) =  (0, Type_void, [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success D"
let _ = if (Type_void, []) =  (Type_void, [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success E"
let _ = if ([], []) =  ([], [Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success F"
let _ = if ([], 3) =  ([], 4)  then report_failure () else System.Console.Error.WriteLine "success G"
let _ = if ([]) =  ([Type_other1 3])  then report_failure () else System.Console.Error.WriteLine "success H"

let _ = 
  if !failures then (System.Console.Out.WriteLine "Test Failed"; exit 1) 
  else
      (System.Console.Out.WriteLine "Test Passed"; 
       printf "TEST PASSED OK"; 
       exit 0)

