// #Conformance #ObjectConstructors 
#if TESTS_AS_APP
module Core_longnames
#endif
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

(* Some test expressions *)
[<AbstractClass; Sealed>]
type MyMath() =
    static member Min(a: double, b: double) = System.Math.Min(a, b)
    static member Min(a: int, b: int) = System.Math.Min(a, b)

[<AbstractClass; Sealed; AutoOpen>]
type AutoOpenMyMath() =
    static member AutoMin(a: double, b: double) = System.Math.Min(a, b)
    static member AutoMin(a: int, b: int) = System.Math.Min(a, b)

[<AbstractClass; Sealed; RequireQualifiedAccess>]
type NotAllowedToOpen() =
    static member QualifiedMin(a: double, b: double) = System.Math.Min(a, b)
    static member QualifiedMin(a: int, b: int) = System.Math.Min(a, b)

module OpenSystemMathOnce = 

    open System.Math
    let x = Min(1.0, 2.0)
    test "vwejhweoiu" (x = 1.0)


module OpenSystemMathTwice = 

    open System.Math
    let x = Min(1.0, 2.0)

    open System.Math
    let x2 = Min(2.0, 1.0)

    test "vwejhweoiu2" (x2 = 1.0)

module OpenMyMathOnce = 

    open MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)

    test "vwejhweoiu2" (x = 1.0)
    test "vwejhweoiu3" (x2 = 1)

module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)

    test "vwejhweoiu2" (x = 1.0)
    test "vwejhweoiu3" (x2 = 1)

module OpenAutoMath = 
    open AutoOpenMyMath
    //open NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)

    test "vwejhweoiu2" (x = 1.0)
    test "vwejhweoiu3" (x2 = 1)

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
