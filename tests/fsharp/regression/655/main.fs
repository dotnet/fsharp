#indent "off"

open DF
let x  = new RawPcapfile()
let xI = x :> Datafile

let _ = 
      (System.Console.Out.WriteLine "Test Passed"; 
       System.IO.File.WriteAllText("test.ok", "ok"); 
       exit 0)

	

