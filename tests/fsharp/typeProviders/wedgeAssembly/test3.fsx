
#r "provider.dll"
#if SIGS
#r "test2a-with-sig.dll"
#r "test2b-with-sig.dll"
#else
#if SIGS_RESTRICTED
#r "test2a-with-sig-restricted.dll"
#r "test2b-with-sig-restricted.dll"
#else
#r "test2a.dll"
#r "test2b.dll"
#endif
#endif


let mutable failures = []
let reportFailure s = 
  stdout.WriteLine "\n................TEST FAILED...............\n"; failures <- failures @ [s]

let check s e r = 
  if r = e then  stdout.WriteLine (s+": YES") 
  else (stdout.WriteLine ("\n***** "+s+": FAIL\n"); reportFailure s)

let test s b = 
  if b then ( (* stdout.WriteLine ("passed: " + s) *) ) 
  else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)

(*========================================================================*)

module ReferenceNonGenerativeTypesAcrossAssemblies = 

    let f() : FSharp.HelloWorld.HelloWorldType = Unchecked.defaultof<_>
    let testTypeEquiv(a:FSharp.HelloWorld.HelloWorldType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)


    check "jcenewnwe091q" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092w" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092e" (Test2a.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091r" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092t" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092y" (Test2a.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

    check "jcenewnwe091u" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092i" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092o" (Test2b.ErasedTypes.testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091p" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092a" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092s" (Test2b.ErasedTypes.testTypeEquiv(Test2b.ErasedTypes.f(), f())) true


    check "jcenewnwe091d" (testTypeEquiv(Test2a.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092f" (testTypeEquiv(Test2a.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092g" (testTypeEquiv(Test2a.ErasedTypes.f(), f())) true

    check "jcenewnwe091h" (testTypeEquiv(Test2b.ErasedTypes.f(), Test2a.ErasedTypes.f())) true
    check "jcenewnwe092j" (testTypeEquiv(Test2b.ErasedTypes.f(), Test2b.ErasedTypes.f())) true
    check "jcenewnwe092k" (testTypeEquiv(Test2b.ErasedTypes.f(), f())) true

(*========================================================================*)

module ReferenceGenerativeTypeAcrossAssemblies = 
    let f2() : Test2a.PublicGenerativeTypes.TheGeneratedType2 = Unchecked.defaultof<_>
    let testTypeEquiv2a(a:Test2a.PublicGenerativeTypes.TheGeneratedType2,b:Test2a.PublicGenerativeTypes.TheGeneratedType2) = (a = b)

    check "jcenewnwe091z" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091x" (Test2a.PublicGenerativeTypes.testTypeEquiv2(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f3())) true

    check "jcenewnwe091c" (testTypeEquiv2a(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091v" (testTypeEquiv2a(Test2a.PublicGenerativeTypes.f2(), Test2a.PublicGenerativeTypes.f3())) true

    let f3() : Test2b.PublicGenerativeTypes.TheGeneratedType2 = Unchecked.defaultof<_>
    let testTypeEquiv2b(a:Test2b.PublicGenerativeTypes.TheGeneratedType2,b:Test2b.PublicGenerativeTypes.TheGeneratedType2) = (a = b)

    check "jcenewnwe091b" (testTypeEquiv2b(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091n" (testTypeEquiv2b(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f3())) true
    check "jcenewnwe091m" (testTypeEquiv2b(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f2())) true
    check "jcenewnwe091Q" (testTypeEquiv2b(Test2b.PublicGenerativeTypes.f2(), Test2b.PublicGenerativeTypes.f3())) true

module ReferenceGenerativeTypeAcrossAssembliesViaOpen = 
    open Test2a.PublicGenerativeTypes
    let f2() : TheGeneratedType2 = Unchecked.defaultof<_>
    let testTypeEquiv2a(a:TheGeneratedType2,b:TheGeneratedType2) = (a = b)

    check "jcenewnwe091W" (testTypeEquiv2(f2(), f2())) true
    check "jcenewnwe091E" (testTypeEquiv2(f2(), f3())) true

    check "jcenewnwe091R" (testTypeEquiv2a(f2(), f2())) true
    check "jcenewnwe091T" (testTypeEquiv2a(f2(), f3())) true

    open Test2b.PublicGenerativeTypes
    let f3() : TheGeneratedType2 = Unchecked.defaultof<_>
    let testTypeEquiv2b(a:TheGeneratedType2,b:TheGeneratedType2) = (a = b)

    check "jcenewnwe091Y" (testTypeEquiv2b(f2(), f2())) true
    check "jcenewnwe091U" (testTypeEquiv2b(f2(), f3())) true
    check "jcenewnwe091I" (testTypeEquiv2b(f2(), f2())) true
    check "jcenewnwe091O" (testTypeEquiv2b(f2(), f3())) true

(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

let _ = 
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)


