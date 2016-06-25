// #Conformance #Interop #Unions 
open Lib

let mutable failures = false
let report_failure () = 
  stderr.WriteLine " NO"; failures <- true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

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

let _ = 
  if failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)
