// #Regression #Conformance #Accessibility #SignatureFiles #Regression #Records
#if TESTS_AS_APP
module Core_anon
#endif

open AnonLib

module Test = 

    let testAccess = (FSharpFriendlyAnonymousObjectsWithoutDotNetReflectionData.data1.X, FSharpFriendlyAnonymousObjectsWithoutDotNetReflectionData.data3.X)

    check "ckwweo" (sprintf "%A" FSharpFriendlyAnonymousObjectsWithoutDotNetReflectionData.data1) "(1)"

module Tests2 = 

    let testAccess = (CSharpCompatAnonymousObjects.data1.X, CSharpCompatAnonymousObjects.data3.X, CSharpCompatAnonymousObjects.data3.Y)
    
    check "ckwweo2" (sprintf "%A" CSharpCompatAnonymousObjects.data1) "<>f__AnonymousType1362829513`1'[System.Int32]"
    
    let _ = (CSharpCompatAnonymousObjects.data1 = CSharpCompatAnonymousObjects.data1)

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

