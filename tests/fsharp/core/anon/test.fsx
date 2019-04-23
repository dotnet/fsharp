// #Regression #Conformance #Accessibility #SignatureFiles #Regression #Records
#if TESTS_AS_APP
module Core_anon
#endif

open AnonLib
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check (s:string) x1 x2 = 
    stderr.Write(s)
    if (x1 = x2) then stderr.WriteLine " OK"
    else (stderr.WriteLine (sprintf "fail, expected %A, got %A" x2 x1); report_failure (s))

module Test = 

    let testAccess = (KindB1.data1.X, KindB1.data3.X)

    check "coijoiwcnkwle2"  (sprintf "%A"  KindB1.data1) "{X = 1;}"

module Tests2 = 

    let testAccess = (KindB2.data1.X, KindB2.data3.X, KindB2.data3.Y)
    
    check "coijoiwcnkwle3"  (sprintf "%A"  KindB2.data1) "{X = 1;}"
    
    let _ = (KindB2.data1 = KindB2.data1)

module MoreTests = 

    let testUseInFunctionSignatureOnly (x: {| X1 : int |}) = ()
    let testUseInReturnSignatureOnly () : {| X2 : int |} = Unchecked.defaultof<_>
    let testUseInTypeOfOnly ()  = typeof<{| X3 : int |}>

    let (x : struct (int * int)) = (3, 4)
    let () = match (struct (3,4)) with (a,b) -> ()

module CrossAssemblyTest = 
    let tests() = 
        check "vrknvio1" (SampleAPI.SampleFunction {| A=1; B = "abc" |}) 4 // note, this is creating an instance of an anonymous record from another assembly.
        check "vrknvio2" (SampleAPI.SampleFunctionAcceptingList [ {| A=1; B = "abc" |}; {| A=2; B = "def" |} ]) [4; 5] // note, this is creating an instance of an anonymous record from another assembly.
        check "vrknvio3" (let d = SampleAPI.SampleFunctionReturningAnonRecd() in d.A + d.B.Length) 4 
        check "vrknvio4" (let d = SampleAPIStruct.SampleFunctionReturningAnonRecd() in d.ToString().Replace("\n","").Replace("\r","")) """{A = 1; B = "abc";}"""
    tests()

module CrossAssemblyTestStruct = 
    let tests() = 
        check "svrknvio1" (SampleAPIStruct.SampleFunction {| A=1; B = "abc" |}) 4 // note, this is creating an instance of an anonymous record from another assembly. The structness is inferred in this case.
        check "svrknvio2" (SampleAPIStruct.SampleFunctionAcceptingList [ {| A=1; B = "abc" |}; {| A=2; B = "def" |} ]) [4; 5] // note, this is creating an instance of an anonymous record from another assembly. The structness is inferred in this case.
        check "svrknvio3" (let d = SampleAPIStruct.SampleFunctionReturningAnonRecd() in d.A + d.B.Length) 4 
    tests()

module CrossAssemblyTestTupleStruct = 
    let tests() = 
        check "svrknvio1" (SampleAPITupleStruct.SampleFunction (1, "abc")) 4 // note, this is creating an instance of an anonymous record from another assembly. The structness is inferred in this case.
        check "svrknvio2" (SampleAPITupleStruct.SampleFunctionAcceptingList [ (1, "abc"); (2, "def") ]) [4; 5] // note, this is creating an instance of an anonymous record from another assembly. The structness is inferred in this case.
        check "svrknvio3" (match SampleAPITupleStruct.SampleFunctionReturningStructTuple() with (x,y) -> x + y.Length) 4 
        check "svrknvio4" (let res = SampleAPITupleStruct.SampleFunctionReturningStructTuple() in match res with (x,y) -> x + y.Length) 4 
    tests()

module TypeNotGeneratedBug = 
    
    let foo (_: obj) = ()
    
    let bar() = foo {| ThisIsUniqueToThisTest6353 = 1 |}
    
module FeasibleEqualityNotImplemented = 
    type R = {| number: int |}
    let e = Event< R>()
    e.Trigger {|number = 3|}
    e.Publish.Add (printfn "%A")    // error

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

