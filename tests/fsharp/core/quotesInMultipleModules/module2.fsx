let a = Module1.Test.bar()
let b = sprintf "%A" (Module1.Test.run())
if a = b then
    stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0
else
    eprintf "FAILED, in-module result %s is different from out-module call %s" a b
    exit 1
