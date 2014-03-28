// #Regression 

let _ = 
  match false with 
  | true -> (System.Console.Out.WriteLine "Test Failed"; exit 1) 
  | false -> 
      (System.Console.Out.WriteLine "Test Passed"; 
       System.IO.File.WriteAllText("test.ok", "ok"); 
       exit 0)

let _ = (System.Console.Out.WriteLine "Test Ended"; exit 100)
