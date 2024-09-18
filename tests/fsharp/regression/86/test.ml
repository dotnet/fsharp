// #Regression 

    
let _ = 
  if '\\' = '\092' & "\\" = "\092" then
      (System.Console.Out.WriteLine "Test Passed"; 
       System.IO.File.WriteAllText("test.ok", "ok"); 
       exit 0)

  else (System.Console.Out.WriteLine "Test Failed"; exit 1) 

