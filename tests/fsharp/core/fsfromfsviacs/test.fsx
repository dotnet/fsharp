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

module TestConsumeOptionalParameter = 
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

module TestConsumeCSharpOptionalParameter = 
    open System
    open CSharpOptionalParameters
    check "csoptional23982f31" (SomeClass.MethodTakingOptionals()) 11
    check "csoptional23982f32" (SomeClass.MethodTakingOptionals(x = 6)) 14
    check "csoptional23982f33" (SomeClass.MethodTakingOptionals(y = "aaaaaa")) 14
    check "csoptional23982f34" (SomeClass.MethodTakingOptionals(d = 8.0)) 14

    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?x = Some 6)) 14
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?y = Some "aaaaaa")) 14
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?d = Some 8.0)) 14
    
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?x = None)) 11
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?y = None)) 11
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionals(?d = None)) 11

    // Check the type inferred for an un-annotated first-class use of the method
    check "csoptional23982f35" (let f = SomeClass.MethodTakingOptionals in ((f : unit -> int) ())) 11

    check "csoptional23982f41" (SomeClass.MethodTakingNullableOptionalsWithDefaults()) 11
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaults(x = 6)) 14 // can provide non-nullable 
    check "csoptional23982f43" (SomeClass.MethodTakingNullableOptionalsWithDefaults(y = "aaaaaa")) 14
    // See https://github.com/fsharp/fslang-suggestions/issues/774#issuecomment-516423841
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaults(x = Nullable 6)) 14 // can provide nullable for legacy
    check "csoptional23982f44" (SomeClass.MethodTakingNullableOptionalsWithDefaults(d = Nullable 8.0)) 14 // can provide nullable for legacy
    
    check "csoptional23982f431" (SomeClass.MethodTakingNullableOptionalsWithDefaults(x = 6)) 14
    check "csoptional23982f442" (SomeClass.MethodTakingNullableOptionalsWithDefaults(d = 8.0)) 14

    // When a C# argument has a default value and is nullable (without a default), using ?x to provide an argument takes type option
    check "csoptional23982f435" (SomeClass.MethodTakingNullableOptionalsWithDefaults(?x = Some 6)) 14
    check "csoptional23982f446" (SomeClass.MethodTakingNullableOptionalsWithDefaults(?d = Some 8.0)) 14

    check "csoptional23982f43E" (SomeClass.MethodTakingNullableOptionalsWithDefaults(?x = None)) -92
    check "csoptional23982f44R" (SomeClass.MethodTakingNullableOptionalsWithDefaults(?d = None)) 6

    // Check the type inferred for an un-annotated first-class use of the method
    check "csoptional23982f45" (let f = SomeClass.MethodTakingNullableOptionalsWithDefaults in ((f : unit -> int) ())) 11

    check "csoptional23982f51" (SomeClass.MethodTakingNullableOptionals()) -3
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionals(x = 6)) 4 // can provide nullable for legacy
    check "csoptional23982f53" (SomeClass.MethodTakingNullableOptionals(y = "aaaaaa")) 4
    check "csoptional23982f54" (SomeClass.MethodTakingNullableOptionals(d = 8.0)) 6 
    // See https://github.com/fsharp/fslang-suggestions/issues/774#issuecomment-516423841
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionals(x = Nullable 6)) 4 // can provide nullable for legacy
    check "csoptional23982f54" (SomeClass.MethodTakingNullableOptionals(d = Nullable 8.0)) 6 // can provide nullable for legacy

    check "csoptional23982f523" (SomeClass.MethodTakingNullableOptionals(x = 6)) 4
    check "csoptional23982f544" (SomeClass.MethodTakingNullableOptionals(d = 8.0)) 6
    
    // When a C# argument has a default value and is nullable (without a default), using ?x to provide an argument takes type option
    check "csoptional23982f527" (SomeClass.MethodTakingNullableOptionals(?x = Some 6)) 4
    check "csoptional23982f548" (SomeClass.MethodTakingNullableOptionals(?d = Some 8.0)) 6

    check "csoptional23982f52T" (SomeClass.MethodTakingNullableOptionals(?x = None)) -3
    check "csoptional23982f54Y" (SomeClass.MethodTakingNullableOptionals(?d = None)) -3

    // Check the type inferred for an un-annotated first-class use of the method
    check "csoptional23982f55" (let f = SomeClass.MethodTakingNullableOptionals in ((f : unit -> int) ())) -3


    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", 8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", Nullable 8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", Nullable ())) 11
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable (), "aaaaaa", 8.0)) 13
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable 6, "aaaaaa", 8.0)) 20

    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", d=8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", d=Nullable 8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, "aaaaaa", d=Nullable ())) 11
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable (), "aaaaaa", d=8.0)) 13
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable 6, "aaaaaa", d=8.0)) 20

    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=Nullable 8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=Nullable ())) 11
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable (), y="aaaaaa", d=8.0)) 13
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable 6, y="aaaaaa", d=8.0)) 20

    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=Nullable 8.0)) 20
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(6, y="aaaaaa", d=Nullable ())) 11
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable (), y="aaaaaa", d=8.0)) 13
    check "acsoptional23982f51" (SomeClass.MethodTakingNullables(Nullable 6, y="aaaaaa", d=8.0)) 20

    // Check the type inferred for an un-annotated first-class use of the method
    check "acsoptional23982f55" (let f = SomeClass.MethodTakingNullables in ((f : Nullable<int> * string * Nullable<double> -> int) (Nullable 1,"aaaa",Nullable 3.0))) 8


// This tests overloaded variaitons of the methods, where the overloads vary by type but not nullability
//
// The CHECK_ERRORS cases are not execpted to compile
module TestConsumeCSharpOptionalParameterOverloads = 
    open System
    open CSharpOptionalParameters

    check "csoptional23982f34o" (SomeClass.OverloadedMethodTakingOptionals(d = 8.0)) 14


    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?d = Some 8.0)) 14
    
    check "csoptional23982f42o" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(x = 6)) 14 // can provide non-nullable 

    // See https://github.com/fsharp/fslang-suggestions/issues/774#issuecomment-516423841
    check "csoptional23982f42o" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(x = Nullable 6)) 14 // can provide nullable for legacy

    check "csoptional23982f431o" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(x = 6)) 14

    // When a C# argument has a default value and is nullable (without a default), using ?x to provide an argument takes type option
    check "csoptional23982f435o" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x = Some 6)) 14

    check "csoptional23982f52o" (SomeClass.OverloadedMethodTakingNullableOptionals(x = 6)) 4 // can provide nullable for legacy

    // See https://github.com/fsharp/fslang-suggestions/issues/774#issuecomment-516423841
    check "csoptional23982f52o" (SomeClass.OverloadedMethodTakingNullableOptionals(x = Nullable 6)) 4 // can provide nullable for legacy

    check "csoptional23982f523o" (SomeClass.OverloadedMethodTakingNullableOptionals(x = 6)) 4
    
    check "csoptional23982f52o1" (SomeClass.OverloadedMethodTakingNullables(6, "aaaaaa", 8.0)) 20 // can provide non-nullable
    check "csoptional23982f52o2" (SomeClass.OverloadedMethodTakingNullables(Nullable(6), "aaaaaa", 8.0)) 20 // can provide nullable 
    check "csoptional23982f52o3" (SomeClass.OverloadedMethodTakingNullables(Nullable(6), "aaaaaa", Nullable(8.0))) 20 // can provide nullable 


#if CHECK_ERRORS
    // in these cases there's not enough information to resolve the overload
    check "csoptional23982f31o" (SomeClass.OverloadedMethodTakingOptionals()) 11
    check "csoptional23982f32o" (SomeClass.OverloadedMethodTakingOptionals(x = 6)) 14
    check "csoptional23982f33o" (SomeClass.OverloadedMethodTakingOptionals(y = "aaaaaa")) 14
    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?x = Some 6)) 14
    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?y = Some "aaaaaa")) 14
    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?x = None)) 11
    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?y = None)) 11
    check "csoptional23982f3ao" (SomeClass.OverloadedMethodTakingOptionals(?d = None)) 11

    // Check the type inferred for an un-annotated first-class use of the method
    check "csoptional23982f35o" (let f = SomeClass.OverloadedMethodTakingOptionals in ((f : unit -> int) ())) 11

    check "csoptional23982f41ox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults()) 11
    check "csoptional23982f43ox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(y = "aaaaaa")) 14
    check "csoptional23982f44ox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(d = Nullable 8.0)) 14 // can provide nullable for legacy
    check "csoptional23982f442ox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(d = 8.0)) 14
    check "csoptional23982f446ox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?d = Some 8.0)) 14
    check "csoptional23982f43Eox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x = None)) -92
    check "csoptional23982f44Rox" (SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?d = None)) 6
    // Check the type inferred for an un-annotated first-class use of the method
    check "csoptional23982f45ox" (let f = SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults in ((f : unit -> int) ())) 11

    check "csoptional23982f51o" (SomeClass.OverloadedMethodTakingNullableOptionals()) -3
    check "csoptional23982f53o" (SomeClass.OverloadedMethodTakingNullableOptionals(y = "aaaaaa")) 4
    check "soptional23982f54o" (SomeClass.OverloadedMethodTakingNullableOptionals(d = 8.0)) 6 
    check "csoptional23982f54o" (SomeClass.OverloadedMethodTakingNullableOptionals(d = Nullable 8.0)) 6 // can provide nullable for legacy
    check "csoptional23982f544o" (SomeClass.OverloadedMethodTakingNullableOptionals(d = 8.0)) 6
    check "csoptional23982f548o" (SomeClass.OverloadedMethodTakingNullableOptionals(?d = Some 8.0)) 6
    check "csoptional23982f52To" (SomeClass.OverloadedMethodTakingNullableOptionals(?x = None)) -3
    check "csoptional23982f54Yo" (SomeClass.OverloadedMethodTakingNullableOptionals(?d = None)) -3
    check "csoptional23982f55o" (let f = SomeClass.OverloadedMethodTakingNullableOptionals in ((f : unit -> int) ())) -3

    check "dcsoptional23982f544o" (SomeClass.OverloadedMethodTakingNullables(x= Nullable(), "aaaa" d = Nullable())) 6
    check "dcsoptional23982f55o" (let (f: Nullable<_> * string * Nullable<_> -> int) = SomeClass.OverloadedMethodTakingNullables in f  (Nullable(), "aaa", Nullable())) -3
#endif

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

module TestStructs =
    open StructTests

    let someFunc (s: NonReadOnlyStruct) = 
        s.M(456)
        s.X

    let someByrefFunc (s: byref<NonReadOnlyStruct>) = 
        s.M(456)
        s.X

    let someInrefFunc (s: inref<NonReadOnlyStruct>) = 
        s.M(456)
        s.X

    let someFuncReturn (s: NonReadOnlyStruct) =
        s.X

    let someInrefFuncReturn (s: inref<NonReadOnlyStruct>) =
        s.X

    let test1 () =
        let s = NonReadOnlyStruct()
        check "hdlcjiklhen1" s.X 0
        s.M(123)
        check "hdlcjiklhen2" s.X 123
        check "hdlcjiklhen3" (someFunc s) 456
        check "hdlcjiklhen4" s.X 123


    let test2 () =
        let mutable s = NonReadOnlyStruct()
        check "hdlcjiklhen5" s.X 0
        s.M(123)
        check "hdlcjiklhen6" s.X 123
        check "hdlcjiklhen7" (someByrefFunc &s) 456
        check "hdlcjiklhen8" s.X 456


    let test3 () =
        let s = NonReadOnlyStruct()
        check "hdlcjiklhen9" s.X 0
        s.M(123)
        check "hdlcjiklhen10" s.X 123
        check "hdlcjiklhen11" (someInrefFunc &s) 123
        check "hdlcjiklhen12" s.X 123

    let test4 () =
        let s = NonReadOnlyStruct()
        check "hdlcjiklhen13" s.X 0
        s.M(123)
        check "hdlcjiklhen14" s.X 123
        check "hdlcjiklhen15" (someFuncReturn s) 0 // Technically a bug today, but test is to verify current behavior.
        check "hdlcjiklhen16" s.X 123

    let test5 () =
        let s = NonReadOnlyStruct()
        check "hdlcjiklhen17" s.X 0
        s.M(123)
        check "hdlcjiklhen18" s.X 123
        check "hdlcjiklhen19" (someInrefFuncReturn &s) 123
        check "hdlcjiklhen20" s.X 123

TestStructs.test1 ()
TestStructs.test2 ()
TestStructs.test3 () 
TestStructs.test4 () 
TestStructs.test5 () 

#endif

module TestConsumeCSharpOptionalParameterOverloads_ByNullability = 
    open System
    open CSharpOptionalParameters
    check "cenwceoweioij1" (SomeClass.SimpleOverload()) 203
    check "cenwceoweioij2" (SomeClass.SimpleOverload(6)) 206
    check "cenwceoweioij3" (SomeClass.SimpleOverload(x=6)) 206
    check "cenwceoweioij4" (SomeClass.SimpleOverload(Nullable(6))) 6
    check "cenwceoweioij5" (SomeClass.SimpleOverload(Nullable())) 100
    check "cenwceoweioij4" (SomeClass.SimpleOverload(x=Nullable(6))) 6
    check "cenwceoweioij5" (SomeClass.SimpleOverload(x=Nullable())) 100

module TestOptionalsAndNullablesInt32ToInt64 = 
    open System
    open CSharpOptionalParameters

    // Check can give 32-bit or 64-bit
    check "csoptional23982f31" (SomeClass.MethodTakingOptionalsInt64()) 11L
    check "csoptional23982f32" (SomeClass.MethodTakingOptionalsInt64(x = 6)) 14L
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionalsInt64(?x = Some 6)) 14L
    check "csoptional23982f32" (SomeClass.MethodTakingOptionalsInt64(x = 6L)) 14L
    check "csoptional23982f3a" (SomeClass.MethodTakingOptionalsInt64(?x = Some 6L)) 14L

    // Check can take 32-bit or 64-bit
    check "csoptional23982f41" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64()) 11L
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = 6)) 14L // can provide non-nullable 
    check "csoptional23982f431" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = 6)) 14L
    check "csoptional23982f435" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(?x = Some 6)) 14L
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = 6L)) 14L // can provide non-nullable 
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = Nullable 6L)) 14L // can provide nullable for legacy
    check "csoptional23982f431" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = 6L)) 14L
    check "csoptional23982f435" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(?x = Some 6L)) 14L

    check "csoptional23982f51" (SomeClass.MethodTakingNullableOptionalsInt64()) -3L
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionalsInt64(x = 6)) 4L // can provide nullable for legacy
    check "csoptional23982f523" (SomeClass.MethodTakingNullableOptionalsInt64(x = 6)) 4L
    check "csoptional23982f527" (SomeClass.MethodTakingNullableOptionalsInt64(?x = Some 6)) 4L
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionalsInt64(x = 6L)) 4L // can provide nullable for legacy
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionalsInt64(x = Nullable 6L)) 4L // can provide nullable for legacy
    check "csoptional23982f523" (SomeClass.MethodTakingNullableOptionalsInt64(x = 6L)) 4L
    check "csoptional23982f527" (SomeClass.MethodTakingNullableOptionalsInt64(?x = Some 6L)) 4L

#if CHECK_ERRORS
    // A 32-bit to 64-bit type directed conversion is not allowed for the legacy support of passing a Nullable as named argument
    check "csoptional23982f42" (SomeClass.MethodTakingNullableOptionalsWithDefaultsInt64(x = Nullable 6)) 14 // can provide nullable for legacy
    check "csoptional23982f52" (SomeClass.MethodTakingNullableOptionalsInt64(x = Nullable 6)) 4 // can provide nullable for legacy
#endif

module Test12414 =
    open System
    type C() =
        member val ContentLength: Nullable<int64> = Nullable<int64>(0L) with get, set

    let c = C()
    c.ContentLength <- 0
    
    check "welewcvwlej1" c.ContentLength (Nullable<int64>(0L))

    c.ContentLength <- 6
    
    check "welewcvwlej2" c.ContentLength (Nullable<int64>(6L))

    c.ContentLength <- Nullable 7L
    
    check "welewcvwlej3" c.ContentLength (Nullable<int64>(7L))

    c.ContentLength <- Nullable<int64>()
    
    check "welewcvwlej4" c.ContentLength (Nullable<int64>())

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
