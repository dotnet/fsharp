// #Conformance #Interop #Unions 
#if !NO_LIB_REFERENCE
open Lib
#endif
open FSharpOptionalTests

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check (s : string) x y = 
    stderr.Write(s)
    if x = y then stderr.WriteLine " OK"
    else report_failure (sprintf "%s: expected %A, got %A" s y x)

#if NO_LIB_REFERENCE // Test for https://github.com/Microsoft/visualfsharp/issues/2453#issuecomment-280946177
module TestExtensions = 
    open CustomExtensions

#else
let r1 = Lib2.r1
let r2 = Lib2.r2

let _ = test "fejio" (r2 =  { recd2field1=3; recd2field2= "a" } )

let rr2 = (Lib2.rr2 : rrecd2)

let _ = test "fejio2" (rr2 =  { rrecd2field2="a"; rrecd2field1= 3 } )

let r3 = (Lib2.r3 : string recd3)

let _ = test "fejio2dw" (r3.recd3field1=4)
let _ = test "fejio2dw" (r3.recd3field2="c")
let _ = test "fejio2dw" (LanguagePrimitives.PhysicalEquality r3.recd3field3  r3)

let _ = test "fejio2dw" (Lib2.li1 = [3])
let _ = test "fejio2dw" (Lib2.lr1 = [r1])
let _ = test "fejio2dw" (Lib2.oi1 = Some 3)
let _ = test "fejio2dw" (Lib2.or1 = Some r1)
let _ = test "fejio2dw" (Lib2.ri1 = ref 3)
let _ = test "fejio2dw" (Lib2.rr1 = ref r1)

let _ = test "structunion3948" (Lib2.u0 = Lib.StructUnionsTests.U0)
let _ = test "structunion3949" (Lib2.u1 = Lib.StructUnionsTests.U1(3))
let _ = test "structunion3949" (Lib2.u2 = Lib.StructUnionsTests.U2(3,4))

let _ = test "structunion3948" (compare Lib2.u0 Lib.StructUnionsTests.U0 = 0)
let _ = test "structunion3949" (compare Lib2.u1 (Lib.StructUnionsTests.U1(3)) = 0)
let _ = test "structunion394a" (compare Lib2.u1 (Lib.StructUnionsTests.U1(4)) = -1)
let _ = test "structunion394b" (compare Lib2.u1 (Lib.StructUnionsTests.U1(2)) = 1)
    
let dt = System.DateTime.Now
let u1a = Lib.NestedStructUnionsTests.U1(dt,"a")
let u1b = Lib.NestedStructUnionsTests.U1(dt,"b")
let u2 = Lib.NestedStructUnionsTests.U2(u1a,u1b)
let _ = test "structunion394b11" (Lib.NestedStructUnionsTests.testPattern1(u2))
let _ = test "structunion394b22" (Lib.NestedStructUnionsTests.testPattern2(u2))
let _ = test "structunion394b33" (Lib.NestedStructUnionsTests.testPattern3(u2))
let _ = test "structunion394b14" (Lib.NestedStructUnionsTests.testPattern1mut(u2))
let _ = test "structunion394b25" (Lib.NestedStructUnionsTests.testPattern2mut(u2))
let _ = test "structunion394b36" (Lib.NestedStructUnionsTests.testPattern3mut(u2))


// F# option implicit converter tests

let testFsOpt() =
    let testOpt (t : 'T option) =
        test (sprintf "fsimplicitconv (%A)" t) (ApiWrapper.ConsumeOptionalParam<'T>(t) = t)

    testOpt(Option<int>.None)
    testOpt(Some 42)

    // check that implicit conversion of optionals does 
    // differentiate between 'null' and 'Some null'
    testOpt(Option<string>.None)
    testOpt(Option<string>.Some null)
    testOpt(Some "")
    testOpt(Some "test")

testFsOpt()


module NestedStructPatternMatchingAcrossAssemblyBoundaries = 
    open Lib.NestedStructUnionsTests

    let testPattern1(u2:U2) = 
        match u2 with
        | U2(u1a,u1b) ->
            match u1a, u1b with 
            | U1(dt1,s1), U1(dt2,s2)  -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern2(u2:U2) = 
        match u2 with
        | U2(U1(dt1,s1),U1(dt2,s2)) -> (dt1 = dt2) 

    let testPattern3(u2:U2) = 
        match u2 with
        | U2(U1(dt1,"a"),U1(dt2,"b")) -> (dt1 = dt2) 

    let testPattern1mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(u1a,u1b) ->
            match u1a, u1b with 
            | U1(dt1,s1), U1(dt2,s2)  -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern2mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(U1(dt1,s1),U1(dt2,s2)) -> (dt1 = dt2) && (s1 = "a") && (s2 = "b")

    let testPattern3mut(u2:U2) = 
        let mutable u2 = u2
        match u2 with
        | U2(U1(dt1,"a"),U1(dt2,"b")) -> (dt1 = dt2) 


    let _ = test "structunion394b1a" (testPattern1(u2))
    let _ = test "structunion394b2b" (testPattern2(u2))
    let _ = test "structunion394b3c" (testPattern3(u2))

    let _ = test "structunion394b1d" (testPattern1mut(u2))
    let _ = test "structunion394b2e" (testPattern2mut(u2))
    let _ = test "structunion394b3f" (testPattern3mut(u2))


let TestAccessibility() = 
     let x = new Newtonsoft.Json.Converters.SomeClass()
     let x2 = new Newtonsoft.Json.Converters.ContainerClass.SomeClass()
     Newtonsoft.Json.Converters.SomeClass.SomeMethod()
     Newtonsoft.Json.Converters.ContainerClass.SomeClass.SomeMethod()

(*
    public Lib.discr1_0 d10a = Lib.discr1_0.MkDiscr1_0_A();
    public Lib.discr1_1 d11a = Lib.discr1_1.MkDiscr1_1_A(3);
    public Lib.discr1_2 d12a = Lib.discr1_2.MkDiscr1_2_A(3,4);
    
    public Lib.discr2_0_0 d200a = Lib.discr2_0_0.MkDiscr2_0_0_A();
    public Lib.discr2_1_0 d210a = Lib.discr2_1_0.MkDiscr2_1_0_A(3);
    public Lib.discr2_0_1 d201a = Lib.discr2_0_1.MkDiscr2_0_1_A();
    public Lib.discr2_1_1 d211a = Lib.discr2_1_1.MkDiscr2_1_1_A(3);
    
    public Lib.discr2_0_0 d200b = Lib.discr2_0_0.MkDiscr2_0_0_B();
    public Lib.discr2_1_0 d210b = Lib.discr2_1_0.MkDiscr2_1_0_B();
    public Lib.discr2_0_1 d201b = Lib.discr2_0_1.MkDiscr2_0_1_B(3);
    public Lib.discr2_1_1 d211b = Lib.discr2_1_1.MkDiscr2_1_1_B(4);

    public List<Lib.recd1> r1 = List<int>.MkCons(3,List<int>.MkNil());

*)

module TestExtensions = 
    open CustomExtensions
    check "dfeweeon" (r1.ExtendFSharpType()) 5
    check "dfeweeon" (Lib2().ExtendCSharpType()) 4

    let x = System.DateTime.Now
    check "dfeweeon1" (System.DateTime.Now.ExtendCSharpTypeWithInRefReturnExtension()).Date  x.Date
    check "dfeweeon2" (x.ExtendCSharpTypeWithInRefReturnExtension()).Date x.Date

    let mutable mx = x
    check "dfeweeon4" (mx.ExtendCSharpTypeWithOutRefExtension(); mx) x.Date

    check "dfeweeon5" (mx.ExtendCSharpTypeWithInRefExtension()) x.Year


let ToFSharpFunc() = 
    test "vkejhwew901" (FuncConvert.FromFunc(FSharpFuncTests.ApiWrapper.f1)(3)  =  FSharpFuncTests.ApiWrapper.f1.Invoke(3))
    test "vkejhwew902" (FuncConvert.FromFunc(FSharpFuncTests.ApiWrapper.f2)(3)("a")  =  FSharpFuncTests.ApiWrapper.f2.Invoke(3, "a"))
    test "vkejhwew903" (FuncConvert.FromFunc(FSharpFuncTests.ApiWrapper.f3)(3)("a")(6uy)  =  FSharpFuncTests.ApiWrapper.f3.Invoke(3, "a", 6uy))
    test "vkejhwew904" (FuncConvert.FromFunc(FSharpFuncTests.ApiWrapper.f4)(3)("a")(6uy)(7y)  =  FSharpFuncTests.ApiWrapper.f4.Invoke(3, "a", 6uy, 7y))
    test "vkejhwew905" (FuncConvert.FromFunc(FSharpFuncTests.ApiWrapper.f5)(3)("a")(6uy)(7y)(7s)  =  FSharpFuncTests.ApiWrapper.f5.Invoke(3, "a", 6uy, 7y, 7s))

#endif

#if TESTS_AS_APP
let RUN() = !failures
#else
let _ =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif
