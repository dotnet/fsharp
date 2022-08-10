// Check recursive name resolution
module rec Test2

open Test2.M // the name "Test2" should be in scope, and its nested modules should be accessible

module N = 
    let x = C()

module M = 
    [<Sealed>]
    type C() =
        member x.P = C()

module UnionTestsWithSignature = 
    // Check accessing the *.Is* properties of unions through a signature
    let a = Hello.Goodbye.A

    Hello.Goodbye.Utils.test "vwehlevw1" a.IsA
    Hello.Goodbye.Utils.test "vwehlevw2" (not a.IsB)
    Hello.Goodbye.Utils.test "vwehlevw3" (not a.IsC)

#if TESTS_AS_APP
    let RUN() = !failures
#else
    let aa =
      if not (!Hello.Goodbye.Utils.failures).IsEmpty then 
          stdout.WriteLine "Test Failed"
          exit 1
      else   
          stdout.WriteLine "Test Passed"
          System.IO.File.WriteAllText("test.ok","ok")
          exit 0
#endif

