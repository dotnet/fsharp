// Check recursive name resolution
module rec Test2

open Test2.M // the name "Test2" should be in scope, and its nested modules should be accessible

module N = 
    let x = C()

module M = 
    [<Sealed>]
    type C() =
        member x.P = C()


let _ = 
    if !Hello.Goodbye.Utils.failures then (stdout.WriteLine "Test Failed"; exit 1) 
    else (stdout.WriteLine "Test Passed"; 
            System.IO.File.WriteAllText("test.ok","ok"); 
            exit 0)


