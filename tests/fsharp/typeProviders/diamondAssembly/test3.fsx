
#r "provider.dll"
#r "test1.dll"
#r "test2a.dll"
#r "test2b.dll"
#load "./test1.fsx"
#load "./test2a.fsx"
#load "./test2b.fsx"


let mutable failures = []
let reportFailure s = 
  stdout.WriteLine "\n................TEST FAILED...............\n"; failures <- failures @ [s]

let check s e r = 
  if r = e then  stdout.WriteLine (s^": YES") 
  else (stdout.WriteLine ("\n***** "^s^": FAIL\n"); reportFailure s)

let test s b = 
  if b then ( (* stdout.WriteLine ("passed: " + s) *) ) 
  else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)
(*========================================================================*)


module CheckCrossAssemblyEquivalenceOfErasedTypes = 
    let f() : FSharp.HelloWorld.HelloWorldType = Unchecked.defaultof<_>
    let testTypeEquiv(a:FSharp.HelloWorld.HelloWorldType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)
    check "jcenewnwe091" (Test1.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test1.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test1.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test1.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test1.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test1.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test1.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test1.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2a.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2a.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv2(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (Test2b.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (Test2b.ErasedTypes.testTypeEquiv2(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (testTypeEquiv(Test2a.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091" (testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe091" (testTypeEquiv(Test2b.ErasedTypes.f(), Test1.ErasedTypes.f())) true
    check "jcenewnwe092" (testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092" (testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

module CheckCrossAssemblyEquivalenceOfGeneratedTypes = 
    type TheGeneratedType5 = Test1.PublicGenerativeTypes.TheGeneratedType5

    let f2() : TheGeneratedType5 = Unchecked.defaultof<_>
    let f3() : TheGeneratedType5 = Unchecked.defaultof<_>
    let testTypeEquiv(a:TheGeneratedType5,b:TheGeneratedType5) = (a = b)

    check "jcenewnwe091" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test1.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (Test2b.PublicGenerativeTypes.testTypeEquiv2(Test2b.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (testTypeEquiv(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (testTypeEquiv(Test2a.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (testTypeEquiv(Test2a.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (testTypeEquiv(Test2a.PublicGenerativeTypes.f2(), f2())) true

    check "jcenewnwe091" (testTypeEquiv(Test2b.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091" (testTypeEquiv(Test2b.PublicGenerativeTypes.f2(), Test1.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (testTypeEquiv(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe092" (testTypeEquiv(Test2b.PublicGenerativeTypes.f2(), f2())) true


(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

let _ = 
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)


