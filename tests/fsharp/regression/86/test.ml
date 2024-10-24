// #Regression 

    
let _ = 
  if '\\' = '\092' & "\\" = "\092" then
      (System.Console.Out.WriteLine "Test Passed"; 
       printf "TEST PASSED OK"; 
       exit 0)

  else (System.Console.Out.WriteLine "Test Failed"; exit 1) 

