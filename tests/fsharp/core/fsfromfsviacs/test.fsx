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
