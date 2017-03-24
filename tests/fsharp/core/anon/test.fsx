// #Regression #Conformance #Accessibility #SignatureFiles #Regression #Records
#if TESTS_AS_APP
module Core_anon
#endif

open AnonLib

module Test = 

    let testAccess = (KindB1.data1.X, KindB1.data3.X)

    check "coijoiwcnkwle2"  (sprintf "%A"  KindB1.data1) "{X = 1;}"

module Tests2 = 

    let testAccess = (KindB2.data1.X, KindB2.data3.X, KindB2.data3.Y)
    
    check "coijoiwcnkwle3"  (sprintf "%A"  KindB2.data1) "{X = 1;}"
    
    let _ = (KindB2.data1 = KindB2.data1)

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

