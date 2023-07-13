// #Conformance #Attributes #Interop #Regression 
(*-------------------------------------------------------------------------
!* attribute tests
 *------------------------------------------------------------------------- *)
#if TESTS_AS_APP
module Core_attributes
#endif
#light

#if !TESTS_AS_APP && !NETCOREAPP
#load "testlib.fsi" "testlib.fs" // a warning is expected here
#endif

#if !TESTS_AS_APP && !NETCOREAPP
#r "cslib.dll"
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

let check (s:string) e r = 
  if r = e then  stdout.WriteLine (s+": YES") 
  else (stdout.Write ("\n***** "+s+": FAIL: "); 
        printfn "Expected '%A', Got '%A'" r e
        report_failure s)

open System
open System.Diagnostics

(* ATTRIBUTES *)

#if !TESTS_AS_APP && !NETCOREAPP
[<LoaderOptimization(LoaderOptimization.MultiDomainHost)>] 
#endif

let main = ()

    
(* attribute on a type *)
type [< Obsolete("testing an obsolete warning is printed")>] x = X
      
(* attribute on a type *)
type [<Obsolete("testing an obsolete warning is printed")>] x2 = X2

let [<Obsolete("testing an obsolete warning is printed for a method")>] fx2 (x:x2) = ()

let fx3 (x:x2) = fx2 x

(* attribute on a method *)
let [<Obsolete("DEBUG")>] myLoggingMethod x = stderr.WriteLine(x:string)

#if !NETCOREAPP
let [<STAThread>] myLoggingMethod2 x = stderr.WriteLine(x:string)
#endif

(* attribute on a type *)
type [<Obsolete("DEBUG")>] y = Y

type y2 = | [<Obsolete("Freddie")>]  Y | [<Obsolete("Mercury")>]  Z

let y2f () = (Y,Z)

(* attribute on a field *)
type record = 
    { myField1: int;
      [<Obsolete("ABBA")>] myField2: int;
      [<Obsolete("DEBUG")>] myDebugField3: string }

let recordf x = x.myField2

(*
#r "System.Security.dll";;
#r "System.Configuration.dll";;
open System.Configuration
type CustomSection = 
class
  inherit ConfigurationSection 
  val fileName : string
  new() = {inherit ConfigurationSection(); fileName = ""}
  [<ConfigurationProperty("fileName", DefaultValue= "")>]
  member m.FileName = m.fileName
end;;
*)

(* attribute on a method parameter *)
(* NOT YET: let myMethod2 x ([<Obsolete("PARAM2")>] y) = stderr.WriteLine x *)

(* attribute on an instance property type *)
(* NOT YET: let [<Obsolete("DEBUG")>] x.myDebug = x.myDebugField3 *)

(* attribute on a return type *)
(* NOT YET: let [<return: Ignore("ignore")>] myMethod3 x = x + 1 *)


type A =
    class
         [<System.Obsolete("The Shock of it All!")>] new() = { }
         [<System.Obsolete("M is old hat. Try N")>] member this.M() = ()
         [<System.Obsolete("Hello")>] static member M2() = ()
    end

let ww (x: A) = ()
let xx = new A()
let yy = xx.M()
let zz = A.M2()

let checkAttributeCount s attrs n = 
    if Array.length attrs <> n then report_failure (sprintf "incorrect number of CAs on '%s', expected %d, got %d" s n attrs.Length)

module CheckAttributesExist = begin
    open System.Reflection

    [<System.Obsolete("Don't use this module!")>] 
    module Outer = begin
       type Inner = A | B
       let x = 1
       let ty = typeof<Inner> 
       do checkAttributeCount "outer module" (ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>, false)) 1;
    end
end

module CheckNullAttributeOnString = begin

    [<System.Obsolete(null)>] 
    module Outer = begin
       type Inner = A | B
       let x = 1
       let ty = typeof<Inner> 
       do checkAttributeCount "property f1" (ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)) 1;
       do match ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false) with 
          | [| (:? System.ObsoleteAttribute as ca)  |]  -> check "test423cwo3ne02" ca.Message null
          | _ -> check "no attribute found" true false
    end

    [<System.Obsolete("")>] 
    module Outer2 = begin
       type Inner = A | B
       let x = 1
       let ty = typeof<Inner> 
       do checkAttributeCount "property f1" (ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)) 1;
       do match ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false) with 
          | [| (:? System.ObsoleteAttribute as ca)  |]  -> check "test423cwo3ne02" ca.Message ""
          | _ -> check "no attribute found" true false
    end
    
    let v = Outer2.Inner.A

    [<Literal>]
    let NullLiteral : string = null
    
    [<System.Obsolete(NullLiteral)>] 
    module Outer3 = begin
       type Inner = A | B
       let x = 1
       let ty = typeof<Inner> 
       do checkAttributeCount "property f1" (ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)) 1;
       do match ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false) with 
          | [| (:? System.ObsoleteAttribute as ca)  |]  -> check "test423cwo3ne02" ca.Message null
          | _ -> check "no attribute found" true false
    end
    
    [<Literal>]
    let NullLiteral1 : string = null
    
    [<Literal>]
    let NullLiteral2 : string = NullLiteral1
    
    [<System.Obsolete(NullLiteral2)>] 
    module Outer4 = begin
       type Inner = A | B
       let x = 1
       let ty = typeof<Inner> 
       do checkAttributeCount "property f1" (ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)) 1;
       do match ty.DeclaringType.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false) with 
          | [| (:? System.ObsoleteAttribute as ca)  |]  -> check "test423cwo3ne02" ca.Message null
          | _ -> check "no attribute found" true false
    end
    
    
    
end

open System

/// Test the use of '|||' in attributes
[<System.AttributeUsageAttribute(System.AttributeTargets.Assembly ||| System.AttributeTargets.Class) >]
type BitwiseOrAttribute = class
    inherit Attribute
end

[<System.AttributeUsageAttribute(System.AttributeTargets.Assembly ||| System.AttributeTargets.Class ||| System.AttributeTargets.Delegate) >]
type BitwiseOrAttribute2 = class
    inherit Attribute
end
           
[<System.Reflection.AssemblyTitle("My Assembly")>]
do ()

type dummy = Dummy
let assembly = typeof<dummy>.Assembly 

// Assembly attributes are currently ignored by F# Interactive, so this test
// fails.  We ignore the failure.
#if COMPILED
let ca = assembly.GetCustomAttributes(typeof<System.Reflection.AssemblyTitleAttribute>,false)

for item in ca do
    printfn "%A" ((item :?> System.Reflection.AssemblyTitleAttribute).Title)
do if Array.length ca <> 1 then failwith "could not find CA on assembly"
#endif

[<Obsolete("Really?")>] 
type y3 = Y3

let ca3 = typeof<y3>.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)
do if Array.length ca3 <> 1 then failwith "could not find CA on type"

[<ObsoleteAttribute("Really?")>] 
type y4 = Y4

let ca4 = typeof<y4>.GetCustomAttributes(typeof<System.ObsoleteAttribute>,false)
do if Array.length ca4 <> 1 then failwith "could not find CA on type"


#if !NETCOREAPP
open System.Runtime.InteropServices

[<DllImport("KERNEL32.DLL", EntryPoint="MoveFileW",  SetLastError=true,CharSet=CharSet.Unicode, ExactSpelling=true,CallingConvention=CallingConvention.StdCall)>]
let MoveFile ((src : string), (dst: string)) : bool = failwith "extern"
#endif

//---------------------------------------------------------------------
// Test we can define an attribute that accepts a named property argument


type DontPressThisButtonAttribute = 
  class 
    inherit System.Attribute
    val v: string 
    val mutable someOtherField: string 
    member x.SomeOtherField 
       with get() = x.someOtherField 
       and  set(v:string) = x.someOtherField <- v
    member x.Message = x.v
    new(s:string) = { inherit System.Attribute(); v=s; someOtherField="" }
  end

type [<DontPressThisButton("Please don't press this again",SomeOtherField="nor me")>] button = Buttpon

let ca5 = typeof<button>.GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)
do if Array.length ca5 <> 1 then report_failure (sprintf "could not get parameterized CA on type, num CAs = %d" (Array.length ca5))
do if (Array.get ca5 0 :?> DontPressThisButtonAttribute).SomeOtherField <> "nor me" then failwith "unexpected value found on parameterized CA on type"


//---------------------------------------------------------------------
// Test we can define an attribute that accepts a named field argument
//

type DontPressThisButton2Attribute = 
  class 
    inherit System.Attribute
    val v: string 
    val mutable SomeOtherField: string 
    member x.Message = x.v
    new(s:string) = { inherit System.Attribute(); v=s; SomeOtherField="" }
  end

type [<DontPressThisButton2("Please don't press this again",SomeOtherField="nor me again")>] button2 = Buttpon2

let ca6 = typeof<button2>.GetCustomAttributes(typeof<DontPressThisButton2Attribute>,false)
do if Array.length ca6 <> 1 then report_failure (sprintf "could not get parameterized CA on type, num CAs = %d" (Array.length ca6))
do if (Array.get ca6 0 :?> DontPressThisButton2Attribute).SomeOtherField <> "nor me again" then failwith "unexpected value found on parameterized CA on type"

//---------------------------------------------------------------------
// Test we can define an attribute that accepts objects and/or 
// a negative integer
//


[<System.AttributeUsage (System.AttributeTargets.All,AllowMultiple=true)>]  
type DontPressThisButton3Attribute = 
  class 
    inherit System.Attribute
    val v: int 
    val obj1: obj
    val obj2: obj
    member x.Number = x.v
    member x.Object1 = x.obj1
    member x.Object2 = x.obj2
    new(n,obj1,obj2) = { inherit System.Attribute(); v=n; obj1= obj1; obj2=obj2  }
  end

type [<DontPressThisButton3(-1, "", -2)>] button3 = Buttpon3

let ca7 = typeof<button3>.GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
do if Array.length ca7 <> 1 then report_failure (sprintf "could not get parameterized CA on type, num CAs = %d" (Array.length ca7))
do if (Array.get ca7 0 :?> DontPressThisButton3Attribute).Number <> -1 then failwith "unexpected value found on parameterized CA on type"
do if (Array.get ca7 0 :?> DontPressThisButton3Attribute).Object1 <> box ("") then failwith "unexpected value found on parameterized CA on type (2)"
do if (Array.get ca7 0 :?> DontPressThisButton3Attribute).Object2    <> box (-2) then failwith "unexpected value found on parameterized CA on type (3)"


[<DontPressThisButton3(1, "", -2)>]
[<type: DontPressThisButton3(2, "", -2)>]
type button4 = Buttpon4
let ca7b = typeof<button4>.GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
do if Array.length ca7b <> 2 then report_failure (sprintf "could not get parameterized CA on type, num CAs = %d" (Array.length ca7b))

[<DontPressThisButton3(1, "", -2);
  DontPressThisButton3(2, "", -2)>]
type button5 = Buttpon5
let ca7c = typeof<button4>.GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
do if Array.length ca7c <> 2 then report_failure (sprintf "could not get parameterized CA on type, num CAs = %d" (Array.length ca7c))

[<assembly: DontPressThisButton3(1, "", -2)>]
do()

let ca7d = 
    let ty = typeof<button4> in
    ty.Assembly.GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
do if Array.length ca7d <> 1 then report_failure (sprintf "could not get parameterized CA on assembly, num CAs = %d" (Array.length ca7d))

#if !NETCOREAPP
#if COMPILED
[<``module``: DontPressThisButton3(1, "", -2)>]
do()

let ca7e = 
    let ty = typeof<button4> in
    ty.Module.GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
do if Array.length ca7e <> 1 then report_failure (sprintf "could not get parameterized CA on module, num CAs = %d" (Array.length ca7e))
#endif
#endif

module AttributesOnUnionCases = begin

    type Cases = 
        | [<DontPressThisButton3(1, "", -2)>] 
          Case1 of int
        | [<DontPressThisButton3(1, "", -2)>] 
          Case2 
          
    let ca7e = 
        let ty = typeof<Cases> in
        ty.GetMethod("NewCase1").GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
        
    do if Array.length ca7e <> 1 then report_failure (sprintf "could not get parameterized CA on non-nullary union case, num CAs = %d" (Array.length ca7e))

    let ca7f = 
        let ty = typeof<Cases> in
        ty.GetMethod("get_Case2").GetCustomAttributes(typeof<DontPressThisButton3Attribute>,false)
        
    do if Array.length ca7f <> 1 then report_failure (sprintf "could not get parameterized CA on nullary union case, num CAs = %d" (Array.length ca7f))

end

module CheckGenericParameterAttibutesAndNames = 
   
    // identical in signature and implementation
    type Cases() = 
        static member M<[<System.CLSCompliantAttribute(true)>] 'T>(x:'T) = x
        static member M2<'U, 'V>(x:'U,y:'V) = x
        static member M3(x) = x

    let ca7e = typeof<Cases>.GetMethod("M").GetGenericArguments().[0].GetCustomAttributes(typeof<System.CLSCompliantAttribute>,false)

    if ca7e.Length <> 1 then report_failure (sprintf "could not get parameterized CA on generic parameter, num CAs = %d" ca7e.Length)

    if typeof<Cases>.GetMethod("M").GetGenericArguments().[0].Name <> "T" then report_failure "wrong name on generic parameter (A)" 
    if typeof<Cases>.GetMethod("M2").GetGenericArguments().[0].Name <> "U" then report_failure "wrong name on generic parameter (B)" 
    if typeof<Cases>.GetMethod("M2").GetGenericArguments().[1].Name <> "V" then report_failure "wrong name on generic parameter (C)" 
    if typeof<Cases>.GetMethod("M3").GetGenericArguments().[0].Name <> "a" then report_failure "unexpected inferred name on generic parameter (D)" 

#if !TESTS_AS_APP && !NETCOREAPP
module CheckAttributesOnElementsWithSignatures = 

    let checkOneAttribute msg (cas: _ []) = 
        if cas.Length <> 1 then report_failure (sprintf "incorrect number of attributes: %s" msg)

    for valName in ["x1"; "x2"; "x3"; "x4"] do 
        checkOneAttribute ("clkjeneew1 - " + valName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("ValAttributesDifferent").GetProperty(valName).GetCustomAttributes(typeof<System.ObsoleteAttribute>,false))

    for tyconName in ["C1"; "C2"; "C3"; "C4"] do 
        checkOneAttribute ("clkjeneew2 - " + tyconName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("TyconAttributesDifferent").GetNestedType(tyconName).GetCustomAttributes(typeof<System.ObsoleteAttribute>,false))

    for moduleName in ["M1"; "M2"; "M3"; "M4"] do 
        checkOneAttribute ("clkjeneew3 - " + moduleName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("ModuleAttributesDifferent").GetNestedType(moduleName).GetCustomAttributes(typeof<System.ObsoleteAttribute>,false))

    for unionCaseTypeName in ["U1"; "U2"; "U3"; "U4"] do 
        checkOneAttribute ("clkjeneew4 - " + unionCaseTypeName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("UnionCaseAttributesDifferent").GetNestedType(unionCaseTypeName).GetMethod("NewA").GetCustomAttributes(typeof<System.ObsoleteAttribute>,false))

    for methodName in ["x1"; "x2"; "x3"; "x4"] do 
        checkOneAttribute ("clkjeneew5 - " + methodName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("ParamAttributesDifferent").GetMethod(methodName).GetParameters().[0].GetCustomAttributes(typeof<System.CLSCompliantAttribute>,false))

    for methodName in ["x1"; "x2"; "x3"; "x4"] do 
        checkOneAttribute ("clkjeneew6 - " + methodName)  (typeof<TestLibModule.ThisLibAssembly>.DeclaringType.GetNestedType("TypeParamAttributesDifferent").GetMethod(methodName).GetGenericArguments().[0].GetCustomAttributes(typeof<System.CLSCompliantAttribute>,false))
#endif

//---------------------------------------------------------------------
// 
module SingleParameterFix = begin


  type C1Attribute = 
      class 
          inherit System.Attribute 
          new() = { } 
          member x.c with set(v:int) = () 
      end
  type C2Attribute = 
      class 
          inherit System.Attribute 
          new(x:int) = { } 
          member x.c with set(v:int) = () 
      end

  [<C1(c=3)>]
  let c1 = ()

  [<C2(3,c=3)>]
  let c2 = ()
end

;;

//---------------------------------------------------------------------
// 


#if !NETCOREAPP

#r "System.Security.dll";;
#r "System.Configuration.dll";;
open System.Configuration;;

type FSharpTestConfig = 
  class
    inherit ConfigurationSection 
    new() = {inherit ConfigurationSection(); } 

    member  m.get_test_string_param2() = (m.Item("test_string_param"):?>string) 
    member  m.get_test_string_param3 = (m.Item("test_string_param"):?>string) 

    [<ConfigurationProperty("foo", DefaultValue = -1)>]
    member  m.get_test_string_param4  
                with  get()  = (m.Item("test_string_param"):?>string) 

    [<ConfigurationProperty("test_string_param",DefaultValue= "teststring")>]
    member  m.test_string_param
              with get()  = (m.Item("test_string_param"):?>string) 
              and  set(v:string) = m.Item("test_string_param") <- v

    [<ConfigurationProperty("test_bool_param",DefaultValue= true)>]
    member  m.test_bool_param
      with get()  = (m.Item("test_bool_param"):?>bool)
      and  set(v:bool) = m.Item("test_bool_param") <- v
  end

module RandomPhilTrelfordTest = begin

    open System.Diagnostics
    open System.Configuration

    type DiagDebug = System.Diagnostics.Debug

    let f() = 
      let config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) in
      let server = config.AppSettings.Settings.get_Item("SMTPServer") in
      printf "%s" (server.Value.ToString());
      DiagDebug.WriteLine("Hello world")
end
#endif

(*-------------------------------------------------------------------------
!* thread static
 *------------------------------------------------------------------------- *)

module ThreadStaticTest = begin

    open System
    open System.Threading 
    open System.Runtime.CompilerServices


    let lock_obj = new Object()


    let safe_print (s:string) = 
        lock lock_obj
            (fun () ->
                stdout.WriteLine s)

    let safe_fail (s:string) = 
        lock lock_obj
            (fun () ->
                stdout.WriteLine s);
        exit 1
     

    type C() = 
        [<ThreadStatic; DefaultValue>]
        static val mutable private n : int
        static member N with get() = C.n and set v = C.n <- v

        [<ThreadStatic; DefaultValue>]
        static val mutable private results : int list
        static member Results with get() = C.results and set v = C.results <- v

#if !MONO && !NETCOREAPP
    let N = 1000
    let main() = 
        let t1 = 
            new Thread(
                fun () -> 
                    C.N <-  0;
                    C.Results <-  [];
                    for x = 1 to N do 
                        C.Results <- C.N :: C.Results;
                        C.N <- 1 + C.N;
                        Thread.Sleep(0)
                    done;
                    safe_print
                        (Printf.sprintf "Thread 1: %s\r\n" (sprintf "%A" C.Results));
                    if List.rev C.Results <> List.init N (fun i -> i) then 
                      safe_fail(Printf.sprintf "Thread 1 failed to produce correct results!\r\n" )
                    ) in
        let t2 = 
            new Thread(
                fun () -> 
                    C.N <- 0;
                    C.Results <-  [];
                    for x = 1 to N do 
                        C.Results <- C.N :: C.Results;
                        C.N <- C.N - 1;
                        Thread.Sleep(0)
                    done;
                    safe_print
                        (Printf.sprintf "Thread 2: %s\r\n" (sprintf "%A" C.Results));
                    if List.rev C.Results <> List.init N (fun i -> -i) then 
                      safe_fail(Printf.sprintf "Thread 2 failed to produce correct results!\r\n")
                    ) in
        t1.Start();
        t2.Start();
        safe_print "Return to end ...";
        t1.Join();
        t2.Join();
        ()
        

    do main()
#endif

end


(*-------------------------------------------------------------------------
!* System.Runtime.InteropServices.In/OUT attributes
 *------------------------------------------------------------------------- *)
#if !NETCOREAPP
open System
let g   ( [<System.Runtime.InteropServices.Out>] x : int byref) = 0
let g2 (( [<System.Runtime.InteropServices.In>]  x : int byref), ([<System.Runtime.InteropServices.Out >] y : int byref)) = 0
let g11 ( [<System.Runtime.InteropServices.In>]  x : int byref)  ([<System.Runtime.InteropServices.Out >] y : int byref)  = 0
#endif

type C = 
  class 
    [<DontPressThisButtonAttribute("no!")>]
    val f1 : int
    [<field: DontPressThisButtonAttribute("no!")>]
    val f2 : int
    [<property: DontPressThisButtonAttribute("no!")>]
    val f3 : int
    [<field: DontPressThisButtonAttribute("no!")>]
    [<property: DontPressThisButtonAttribute("no!")>]
    val f4 : int

    [<DontPressThisButtonAttribute("no!")>]
    val mutable mf1 : int
    [<field: DontPressThisButtonAttribute("no!")>]
    val mutable mf2 : int
    //[<property: DontPressThisButtonAttribute("no!")>]
    //val mutable mf3 : int
    //[<field: DontPressThisButtonAttribute("no!")>]
    //[<property: DontPressThisButtonAttribute("no!")>]
    //val mutable mf4 : int

    static member StaticMember ( [<System.Runtime.InteropServices.Out >] p : int byref) = 0 
    member x.InstanceMember    ( [<System.Runtime.InteropServices.Out >] p : int byref) = 0
    member x.InstanceP 
        with get (idx : int) = ()
        and  set (idx : int) (v:int) = ()

    [<Obsolete("obsolete")>]
    abstract VirtualMethod1 : p : int byref -> int
    [<Obsolete("obsolete")>]
    default this.VirtualMethod1  (p : int byref) = 0

    abstract VirtualMethod2 : p : int byref -> int
    [<Obsolete("obsolete")>]
    default this.VirtualMethod2  (p : int byref) = 0

    [<Obsolete("obsolete")>]
    abstract VirtualMethod3 : p : int byref -> int
    default this.VirtualMethod3  (p : int byref) = 0

    end

#if !NETCOREAPP
let test2179 = 
    let ty = typeof<C> in

    checkAttributeCount "property f1" (ty.GetProperty("f1").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    checkAttributeCount "property f2" (ty.GetProperty("f2").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 0;
    checkAttributeCount "property f3" (ty.GetProperty("f3").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    checkAttributeCount "property f4" (ty.GetProperty("f4").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    let fields = ty.GetFields(Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Public ||| Reflection.BindingFlags.GetField ||| Reflection.BindingFlags.Instance  ||| Reflection.BindingFlags.Static) in
    printfn "fields = %A" fields;
    let findField nm = fields |> Array.find(fun f -> f.Name = nm) in
    checkAttributeCount "field f1@" (findField("f1@").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 0;
    checkAttributeCount "field f2@" (findField("f2@").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    checkAttributeCount "field f3@" (findField("f3@").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 0;
    checkAttributeCount "field f4@" (findField("f4@").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;

    //checkAttributeCount "property mf1" (ty.GetProperty("mf1").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    //checkAttributeCount "property mf2" (ty.GetProperty("mf2").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 0;
    //checkAttributeCount "property mf3" (ty.GetProperty("mf3").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    //checkAttributeCount "property mf4" (ty.GetProperty("mf4").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    checkAttributeCount "field mf1" (ty.GetField("mf1").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    checkAttributeCount "field mf2" (ty.GetField("mf2").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    //checkAttributeCount "field mf3" (ty.GetField("mf3").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;
    //checkAttributeCount "field mf4" (ty.GetField("mf4").GetCustomAttributes(typeof<DontPressThisButtonAttribute>,false)) 1;

    checkAttributeCount "method VirtualMethod1" (ty.GetMethod("VirtualMethod1").GetCustomAttributes(typeof<ObsoleteAttribute>,false)) 1;
    checkAttributeCount "method VirtualMethod2" (ty.GetMethod("VirtualMethod2").GetCustomAttributes(typeof<ObsoleteAttribute>,false)) 1
    //checkAttributeCount "method VirtualMethod3" (ty.GetMethod("VirtualMethod3").GetCustomAttributes(typeof<ObsoleteAttribute>,false)) 1
#endif

let paramsOf (typ:System.Type) (methName:string) =
  let meth = typ.GetMethod(methName) in
  let pars = meth.GetParameters() in
  Array.toList pars

let name (x:System.Reflection.ParameterInfo) n = x.Name = n    

do let [p] = paramsOf (typeof<C>) "get_InstanceP" in
   assert(name p "idx"); printf "ok\n"

do let [p;_] = paramsOf (typeof<C>) "set_InstanceP" in
   assert(name p "idx"); printf "ok\n"

do let [p] = paramsOf (typeof<C>) "StaticMember" in
   assert(p.IsOut && name p "p"); printf "ok\n"

do let [p] = paramsOf (typeof<C>) "StaticMember" in
   assert(p.IsOut && name p "p"); printf "ok\n"

#if LITERALS
module LiteralAttributeTests = begin

    [<Literal>]
    let x = 1
    let get_x = x

    [<Literal>]
    let x2 = 1s
    let get_x2 = x2

    [<Literal>]
    let x3 = 1y
    let get_x3 = x3

    [<Literal>]
    let x4 = 1uy
    let get_x4 = x4

    [<Literal>]
    let x5 = 1us
    let get_x5 = x5

    [<Literal>]
    let x6 = 1u
    let get_x6 = x6

    [<Literal>]
    let x7 = 1L
    let get_x7 = x7

    [<Literal>]
    let x8 = 1UL
    let get_x8 = x8

    [<Literal>]
    let x9 = 'a'
    let get_x9 = x9

    [<Literal>]
    let x10 = 0.1
    let get_x10 = x10

    [<Literal>]
    let x11 = 0.1f
    let get_x11 = x11

    [<Literal>]
    let s = "hello"
    let get_s = s

    let y = x

    let z = x + x

    let z2 = x.CompareTo(box 3)

    [<System.Obsolete(s)>]
    let z3 = 4

    [<System.Obsolete(s)>]
    type T = A | B

    
    type T2 = 
        | [<System.Obsolete(s)>] A 
        | B

    type T3 = 
        { [<System.Obsolete(s)>] r1: int }
    
end
#endif

module BasicStructuralEqHashCompareAttributeChecks = begin

    type R = R of int * int

    let _ = check "whjevoi1" (R(1,1+1) = R(1,2)) true
    let _ = check "whjevoi2" (not (R(1,3) = R(1,2))) true
    let _ = check "whjevoi3" (hash (R(1,1+1)) = hash (R(1,2))) true
    let _ = check "whjevoi4" (R(1,2) < R(1,3))  true
    let _ = check "whjevoi5" (R(1,2) < R(2,3))  true
    let _ = check "whjevoi6" (R(1,2) < R(2,1))  true
    let _ = check "whjevoi7" (R(1,2) > R(1,0))  true

    type R1 = 
        { myData : int }
        with
            static member Create() = { myData = 0 }
        end

    [<ReferenceEquality>]
    type R2 = 
        { mutable myState : int }
        with
            static member Fresh() = { myState = 0 }
        end

    [<ReferenceEquality; NoComparison >]
    type R2b = 
        { mutable myState : int }
        with
            static member Fresh() = { myState = 0 }
        end

    [<StructuralEquality; NoComparison >]
    type R3 = 
        { someType : System.Type }
        with 
            static member Make() = { someType = typeof<int> }
        end


    let _ = check "ce99pj321"  (R1.Create() = R1.Create() ) true
    let _ = check "ce99pj322"  (R1.Create() = R1.Create() ) true
    let _ = check "ce99pj323"  (R2.Fresh() = R2.Fresh()) false
    let _ = check "ce99pj324"  (R2b.Fresh() = R2b.Fresh()) false
    let _ = check "ce99pj325"  (R3. Make() = R3. Make()) true

    // structural comparison raises an exception if not implemented
    let _ = check "ce99pj32e" (try (let _ = Unchecked.compare (R2.Fresh()) (R2.Fresh()) in false) with _ -> true) true
    let _ = check "ce99pj32p" (try (let _ = Unchecked.compare (R2b.Fresh()) (R2b.Fresh()) in false) with _ -> true) true
    let _ = check "ce99pj32j" (try (let _ = Unchecked.compare (R3. Make()) (R3. Make()) in false) with _ -> true) true

end

[<System.Diagnostics.DebuggerTypeProxy(typeof<TestTypeOnTypeView>)>]
type TestTypeOnType() = 
    class
       member x.P = 1
    end
    
and TestTypeOnTypeView() = 
    class
       member x.P = 1
    end


module Bug1437_PS_FSharp1_0_AttributesWithArrayArguments = begin

    [<System.AttributeUsageAttribute(System.AttributeTargets.Assembly) >]
    type AttributeWithArrayArgAttribute(data: int[]) = class
        inherit Attribute()
    end

    [<assembly:AttributeWithArrayArg ([|0;1;2|])>]
    do ()

    let assembly = typeof<AttributeWithArrayArgAttribute>.Assembly 

    // Assembly attributes are currently ignored by F# Interactive, so this test
    // fails.  We ignore the failure.
    #if COMPILED
    let ca = assembly.GetCustomAttributes(typeof<AttributeWithArrayArgAttribute>,false)
    let _ = check "ce99pj32cweq" (Array.length ca) 1 
    #endif
end

module Bug6161_PS_FSharp1_0_MoreAttributesWithArrayArguments = begin

    type IntArrayPropAttribute() = 
        inherit System.Attribute() 
        let mutable attribs = [| |]
        member x.Value with set(v:int[]) = attribs <- v and get() = attribs


    type ObjArrayPropAttribute() = 
        inherit System.Attribute() 
        let mutable attribs = [| |]
        member x.Value with set(v:obj[]) = attribs <- v and get() = attribs

    type AnyArrayPropAttribute() = 
        inherit System.Attribute() 
        let mutable attribs = [| |]
        member x.Value with set(v:obj[]) = attribs <- v and get() = attribs

    type IntArrayAttribute(a:int[]) = 
        inherit System.Attribute()
        member x.Values = a

    type ObjArrayAttribute(a:obj[]) = 
        inherit System.Attribute()
        member x.Values = a

    type AnyAttribute(a:obj) = 
        inherit System.Attribute()
        member x.Value = a

    (* works *)
    [<IntArrayProp(Value = [| 42 |])>]
    [<ObjArrayProp(Value = [| (42 :> obj) |])>]
    [<IntArray [| 42 |]>]
    [<ObjArray [| 42 |]>]
    [<Any [| 42 |]>]
    type T=class end

    let _ = 
        let ty = typeof<T>
        let ca = ty.GetCustomAttributes(typeof<IntArrayPropAttribute>,false)
        check "ce99pj32cweq1" ca.Length 1 
        check "ce99pj32cweq2" (ca.[0].GetType()) (typeof<IntArrayPropAttribute>)
        check "ce99pj32cweq3" (ca.[0] :?> IntArrayPropAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<ObjArrayPropAttribute>,false)
        check "ce99pj32cweq4" ca.Length 1 
        check "ce99pj32cweq5" (ca.[0].GetType()) (typeof<ObjArrayPropAttribute>)
        check "ce99pj32cweq6" (ca.[0] :?> ObjArrayPropAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<IntArrayAttribute>,false)
        check "ce99pj32cweq7" ca.Length 1 
        check "ce99pj32cweq8" (ca.[0].GetType()) (typeof<IntArrayAttribute>)
        check "ce99pj32cweq9" (ca.[0] :?> IntArrayAttribute).Values [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<ObjArrayAttribute>,false)
        check "ce99pj32cweqQ" ca.Length 1 
        check "ce99pj32cweqW" (ca.[0].GetType()) (typeof<ObjArrayAttribute>)
        check "ce99pj32cweqE" (ca.[0] :?> ObjArrayAttribute).Values [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<AnyAttribute>,false)
        check "ce99pj32cweqR" ca.Length 1 
        check "ce99pj32cweqT" (ca.[0].GetType()) (typeof<AnyAttribute>)
        check "ce99pj32cweqY" (ca.[0] :?> AnyAttribute).Value (box [| 42 |])

#if !TESTS_AS_APP && !NETCOREAPP
    let _ = 
        let ty = typeof<CSharpLibrary.TestClass>
        let ca = ty.GetCustomAttributes(typeof<CSharpLibrary.IntArrayPropAttribute>,false)
        check "de89pj32cweq1" ca.Length 1 
        check "de89pj32cweq2" (ca.[0].GetType()) (typeof<CSharpLibrary.IntArrayPropAttribute>)
        check "de89pj32cweq3" (ca.[0] :?> CSharpLibrary.IntArrayPropAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<CSharpLibrary.ObjArrayPropAttribute>,false)
        check "de89pj32cweq4" ca.Length 1 
        check "de89pj32cweq5" (ca.[0].GetType()) (typeof<CSharpLibrary.ObjArrayPropAttribute>)
        check "de89pj32cweq6" (ca.[0] :?> CSharpLibrary.ObjArrayPropAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<CSharpLibrary.IntArrayAttribute>,false)
        check "de89pj32cweq7" ca.Length 1 
        check "de89pj32cweq8" (ca.[0].GetType()) (typeof<CSharpLibrary.IntArrayAttribute>)
        check "de89pj32cweq9" (ca.[0] :?> CSharpLibrary.IntArrayAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<CSharpLibrary.ObjArrayAttribute>,false)
        check "de89pj32cweqQ" ca.Length 1 
        check "de89pj32cweqW" (ca.[0].GetType()) (typeof<CSharpLibrary.ObjArrayAttribute>)
        check "de89pj32cweqE" (ca.[0] :?> CSharpLibrary.ObjArrayAttribute).Value [| 42 |]

        let ca = ty.GetCustomAttributes(typeof<CSharpLibrary.AnyAttribute>,false)
        check "de89pj32cweqR" ca.Length 1 
        check "de89pj32cweqT" (ca.[0].GetType()) (typeof<CSharpLibrary.AnyAttribute>)
        check "de89pj32cweqY" (ca.[0] :?> CSharpLibrary.AnyAttribute).Value (box [| 42 |])
#endif

end


module AttributeParamArrayArgs1 = 

    [<AttributeUsage(AttributeTargets.All)>]
    type AttributeWithParamArray([<ParamArrayAttribute>] Parameters:obj[]) =
      inherit Attribute()

    type Foo1() =
        [<AttributeWithParamArray([| (1 :> obj) |])>]        
        override this.ToString() = "Stuff"

    type Foo2() =
        [<AttributeWithParamArray(1)>]        
        override this.ToString() = "Stuff"

    type Foo3() =
        [<AttributeWithParamArray()>]        
        override this.ToString() = "Stuff"

    type Foo4() =
        [<AttributeWithParamArray(1,2)>]        
        override this.ToString() = "Stuff"

    type Foo5() =
        [<AttributeWithParamArray(1,2,3,4,5,6,7,8)>]        
        override this.ToString() = "Stuff"

module AttributeParamArrayArgs2 = 

    [<AttributeUsage(AttributeTargets.All)>]
    type AttributeWithParamArray(x:string,[<ParamArrayAttribute>] Parameters:obj[]) =
      inherit Attribute()

    type Foo1() =
        [<AttributeWithParamArray("1",[| (1 :> obj) |])>]        
        override this.ToString() = "Stuff"

    type Foo2() =
        [<AttributeWithParamArray("1",1)>]        
        override this.ToString() = "Stuff"

    type Foo3() =
        [<AttributeWithParamArray("1")>]        
        override this.ToString() = "Stuff"

    type Foo4() =
        [<AttributeWithParamArray("1",1,2)>]        
        override this.ToString() = "Stuff"

    type Foo5() =
        [<AttributeWithParamArray("1",1,2,3,4,5,6,7,8)>]        
        override this.ToString() = "Stuff"

module TestImplicitCOnstructorAttribute =
    type Foo [<Obsolete("don't use")>] () =
      member x.Bar() = 1

    checkAttributeCount "Foo.new()" (typeof<Foo>.GetConstructor([| |]).GetCustomAttributes(typeof<ObsoleteAttribute>,false)) 1;

module TestTypeInstantiationsInAttributes =

    type ListProxy<'a>(l:List<'a>) =
        [<DebuggerBrowsableAttribute(DebuggerBrowsableState.RootHidden)>]
        member this.Items = 
            Array.ofList l
            
    [<DebuggerDisplayAttribute("{Length}", Target=typeof<List<int>> )>]
    type C1 = A | B
    [<DebuggerTypeProxyAttribute(typeof<ListProxy<int>>, Target=typeof<List<C1>>)>]
    type C2 = A | B
    [<DebuggerTypeProxyAttribute(typeof<ListProxy<int>>, Target=typeof<List<C1[]>>)>]
    type C3 = A | B
    [<DebuggerTypeProxyAttribute(typeof<ListProxy<int>>, Target=typeof<List<C1>[,]>)>]
    type C4 = A | B
    [<DebuggerTypeProxyAttribute(typedefof<ListProxy<_>>, Target=typedefof<List<_>>)>]
    type C5 = A | B
    
    let attrs1 = typeof<C1>.GetCustomAttributes(typeof<System.Diagnostics.DebuggerDisplayAttribute>,false) ;
    match attrs1 with 
      | [| (:? System.Diagnostics.DebuggerDisplayAttribute as ca)  |]  -> 
          check "test423cwo3nh01a" ca.Value "{Length}"
          check "test423cwo3nh02a" ca.Target typeof<List<int>>
      | _ -> check "no attribute found" true false

    let attrs2 = typeof<C2>.GetCustomAttributes(typeof<System.Diagnostics.DebuggerTypeProxyAttribute>,false) ;
    match attrs2 with 
      | [| (:? System.Diagnostics.DebuggerTypeProxyAttribute as ca)  |]  -> 
#if !MONO
          check "test423cwo3nq01b" ca.ProxyTypeName (typeof<ListProxy<int>>).AssemblyQualifiedName
#endif
          check "test423cwo3nq02b" ca.Target typeof<List<C1>>
      | _ -> check "no attribute found" true false

    let attrs3 = typeof<C3>.GetCustomAttributes(typeof<System.Diagnostics.DebuggerTypeProxyAttribute>,false) ;
    match attrs3 with 
      | [| (:? System.Diagnostics.DebuggerTypeProxyAttribute as ca)  |]  -> 
#if !MONO
          check "test423cwo3nw01c" ca.ProxyTypeName (typeof<ListProxy<int>>).AssemblyQualifiedName
#endif
          check "test423cwo3nw02c" ca.Target typeof<List<C1[]>>
      | _ -> check "no attribute found" true false

    let attrs4 = typeof<C4>.GetCustomAttributes(typeof<System.Diagnostics.DebuggerTypeProxyAttribute>,false) ;
    match attrs4 with 
      | [| (:? System.Diagnostics.DebuggerTypeProxyAttribute as ca)  |]  -> 
#if !MONO
          check "test423cwo3nd01d" ca.ProxyTypeName (typeof<ListProxy<int>>).AssemblyQualifiedName
#endif
          check "test423cwo3nd02d" ca.Target typeof<List<C1>[,]>
      | _ -> check "no attribute found" true false

    let attrs5 = typeof<C5>.GetCustomAttributes(typeof<System.Diagnostics.DebuggerTypeProxyAttribute>,false) ;
    match attrs5 with 
      | [| (:? System.Diagnostics.DebuggerTypeProxyAttribute as ca)  |]  -> 
#if !MONO
          check "test423cwo3ng01e" ca.ProxyTypeName (typedefof<ListProxy<_>>).AssemblyQualifiedName
#endif
          check "test423cwo3ng02e" ca.Target typedefof<List<_>>
      | _ -> check "no attribute found" true false

module NullsInAttributes = 
    open System

    [<AttributeUsage(AttributeTargets.Property)>]
    type NullsAttribute(obj:obj, str:string, typ:System.Type) = 
        inherit Attribute()
        let mutable objprop : obj option = None
        let mutable strprop : string option = None
        let mutable typprop : System.Type option = None

        member x.Object = obj
        member x.String = str
        member x.Type = typ
        member x.ObjectProp with set v = objprop <- Some v
        member x.StringProp with set v = strprop <- Some v
        member x.TypeProp with set v = typprop <- Some v

        member x.ObjectPropValue with get () = objprop 
        member x.StringPropValue with get () = strprop 
        member x.TypePropValue with get () = typprop 

    type TestNullsInAttributes() = 
       [<NullsAttribute(null, null, null)>]
       member x.TestProperty1 = 1
       [<NullsAttribute(null, null, null, ObjectProp=null)>]
       member x.TestProperty2 = 1
       [<NullsAttribute(null, null, null, StringProp=null)>]
       member x.TestProperty3 = 1
       [<NullsAttribute(null, null, null, TypeProp=null)>]
       member x.TestProperty4 = 1
       [<NullsAttribute(null, null, null, ObjectProp=null, StringProp=null, TypeProp=null)>]
       member x.TestProperty5 = 1

       [<NullsAttribute("1", "2", typeof<int16>, ObjectProp="3", StringProp="4", TypeProp=typeof<string>)>]
       member x.TestProperty6 = 1
       
    //let check s b1 b2 = 
    //    if b1 = b2 then printfn "%s OK" s
    //    else printfn "FAIL %s: expected %A, got %A" s b2 b1

    let test p (e1,e2,e3,e4,e5,e6) = 
        check (sprintf "%s.Object" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).Object e1
        check (sprintf "%s.String" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).String e2
        check (sprintf "%s.Type" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).Type e3
        check (sprintf "%s.ObjectProp" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).ObjectPropValue e4
        check (sprintf "%s.StringProp" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).StringPropValue e5
        check (sprintf "%s.TypeProp" p) (typeof<TestNullsInAttributes>.GetProperty(p).GetCustomAttributes(false).[0] :?> NullsAttribute).TypePropValue e6

    test "TestProperty1" (null, null, null, None, None, None)
    test "TestProperty2" (null, null, null, Some null, None, None)
    test "TestProperty3"  (null, null, null, None, Some null, None)
    test "TestProperty4"  (null, null, null, None, None, Some null)
    test "TestProperty5"  (null, null, null, Some null, Some null, Some null)
    test "TestProperty6"  (box "1", "2", typeof<int16>, Some (box "3"), Some  "4", Some typeof<string>)
    
#if !NETCOREAPP
module Bug5762 =
      open System
      open System.IO
      open System.Runtime.ConstrainedExecution
      open System.Runtime.InteropServices
      open System.Security.Permissions
      open Microsoft.Win32.SafeHandles
     
      type LFILETIME = System.Runtime.InteropServices.ComTypes.FILETIME
     
      [<Serializable; StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto); BestFitMapping(false)>]
      type WIN32_FIND_DATA =
        [<DefaultValue>] val mutable dwFileAttributes:FileAttributes
        [<DefaultValue>] val mutable ftCreationTime:LFILETIME
        [<DefaultValue>] val mutable ftLastAccessTime:LFILETIME
        [<DefaultValue>] val mutable ftLastWriteTime:LFILETIME
        [<DefaultValue>] val mutable nFileSizeHigh:uint32
        [<DefaultValue>] val mutable nFileSizeLow:uint32
        [<DefaultValue>] val mutable dwReserved0:uint32
        [<DefaultValue>] val mutable dwReserved1:uint32
        [<DefaultValue; MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)>]
        val mutable cFileName:string
        [<DefaultValue; MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)>]
        val mutable cAlternateFileName:string
        new() = {}
        
      type SafeFindHandle() =
        inherit SafeHandleZeroOrMinusOneIsInvalid(true)
        override this.ReleaseHandle() = true

      type T = class end
      
      [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
      extern SafeFindHandle FindFirstFile(string fileName, [<Out; MarshalAs(UnmanagedType.LPStruct)>] WIN32_FIND_DATA data)
     
      [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
      extern bool FindNextFile(SafeFindHandle hndFindFile, [<Out; MarshalAs(UnmanagedType.LPStruct)>] WIN32_FIND_DATA lpFindFileData)
         
     
      let moduleType = typeof<T>.DeclaringType
      let mFindFirstFile = moduleType.GetMethod("FindFirstFile")
      let dataParam = mFindFirstFile.GetParameters().[1]
      let marshalAsAttrs = dataParam.GetCustomAttributes(typeof<MarshalAsAttribute>, false) |> Array.distinct
      check "gjhfdey547"
        (match marshalAsAttrs with
         | [| (:? MarshalAsAttribute as ma) |] when ma.Value = UnmanagedType.LPStruct -> true
         | _ -> false)
        true   
        
      let findDataType = typeof<WIN32_FIND_DATA>
      check "dguyestgfuysdc"
        (match findDataType.GetField("cFileName").GetCustomAttributes(typeof<MarshalAsAttribute>, false) |> Array.distinct with
         | [| (:? MarshalAsAttribute as ma) |] 
                when ma.Value = UnmanagedType.ByValTStr && ma.SizeConst = 260 ->
                    true
         | _ -> false)
        true
#endif
     
// test CompiledName attribute on properties        
module Bug5936 =
    type T() = 
        let mutable bval = "Boo!"
        
        [<CompiledName "P">]
        static member p = 1
        
        [<CompiledName "M">]
        static member m() = 1
        
        [<CompiledName "IP">]
        member this.ip = 1
        
        [<CompiledName "IM">]
        member this.im() = 1
        
        // The additional applications of CompiledName to the getters and setters won't work
        // since CompiledName has already been applied to the property
        // CompiledName can be applied to the getter, setter or property - it will rename the
        // property itself - you can only use it once
        [<CompiledName "B">]
        member this.b 
            with get() = bval
            and set(v) = bval <- v
            
        member this.c 
            with [<CompiledName "C">] get() = bval
            and set(v) = bval <- v
            
        member this.d 
            with get() = bval
            and [<CompiledName "D">] set(v) = bval <- v

    check "dguyestgfuycntm" (typeof<T>.GetMethod "M" <> null) true
    check "dguyestgfuycntim" (typeof<T>.GetMethod "IM" <> null) true   
    check "dguyestgfuycntp" (typeof<T>.GetProperty "P" <> null) true  
    check "dguyestgfuycntip" (typeof<T>.GetProperty "IP" <> null) true
    check "dguyestgfuycntib" (typeof<T>.GetProperty "B" <> null) true
    check "dguyestgfuycntic" (typeof<T>.GetProperty "C" <> null) true
    check "dguyestgfuycntid" (typeof<T>.GetProperty "D" <> null) true



module AttributeTestsOnExtensionProperties = 
    [<AutoOpen>]
    module Extensions =
        type InlineAttribute(x: string) =
            inherit System.Attribute()
            override x.ToString() = "Inline"

        type MyType() =
            member x.P = 1

        type MyType with
            [<Inline "foo" >]
            member this.ExtensionMethod() = 42
            member this.Item
                with    [<Inline "$this[$field]">]
                        get (field: string) = obj ()

                and     [<Inline "$this[$field]=$value">]
                        set (field: string) (value: obj) = ()

        type MyTypeIntrinsic() =
            member x.P = 1

        type MyTypeIntrinsic with
            [<Inline "foo" >]
            member this.ExtensionMethod() = 42
            member this.Item
                with    [<Inline "$this[$field]">]
                        get (field: string) = obj ()

                and     [<Inline "$this[$field]=$value">]
                        set (field: string) (value: obj) = ()
        type System.Object with
            [<Inline "foo" >]
            member this.ExtensionMethod = 42

            member this.Item
                with    [<Inline "$this[$field]">]
                        get (field: string) = obj ()
                and     [<Inline "$this[$field]=$value">]
                        set (field: string) (value: obj) = ()

    let test0() = 
        match <@ MyType().["foo"] @> with
        | Quotations.Patterns.PropertyGet(_, info, _) ->
            info.DeclaringType.GetMethods()
            |> Seq.map (fun m ->
                sprintf "%s: %A" m.Name (m.GetCustomAttributes(typeof<InlineAttribute>, false)))
            |> Seq.sort
            |> String.concat ", "

        | c -> failwithf "unreachable 1 %A" c

    let test1() = 
        match <@ MyTypeIntrinsic().["foo"] @> with
        | Quotations.Patterns.PropertyGet(_, info, _) ->
            info.DeclaringType.GetMethods()
            |> Seq.map (fun m ->
                sprintf "%s: %A" m.Name (m.GetCustomAttributes(typeof<InlineAttribute>, false)))
            |> Seq.sort
            |> String.concat ", "

        | _ -> failwith "unreachable 1"


    let test2() = 
        typeof<MyType>.GetMethods()
            |> Seq.map (fun m ->
                sprintf "%s: %A" m.Name (m.GetCustomAttributes(typeof<InlineAttribute>, false)))
            |> Seq.sort
            |> String.concat ", "


    let test3() = 
        match <@ obj().["foo"] @> with
        | Quotations.Patterns.Call(_, info, _) ->
            info.DeclaringType.GetMethods()
            |> Seq.map (fun m ->
                sprintf "%s: %A" m.Name (m.GetCustomAttributes(typeof<InlineAttribute>, false)))
            |> Seq.sort
            |> String.concat ", "
        | _ -> failwith "unreachable 3"

 

    check "vwlnwer-0wreknj1" (test0()) "Equals: [||], ExtensionMethod: [|Inline|], GetHashCode: [||], GetType: [||], ToString: [||], get_Item: [|Inline|], get_P: [||], set_Item: [|Inline|]"

    check "vwlnwer-0wreknj2" (test1()) "Equals: [||], ExtensionMethod: [|Inline|], GetHashCode: [||], GetType: [||], ToString: [||], get_Item: [|Inline|], get_P: [||], set_Item: [|Inline|]"

    check "vwlnwer-0wreknj3" (test2()) "Equals: [||], ExtensionMethod: [|Inline|], GetHashCode: [||], GetType: [||], ToString: [||], get_Item: [|Inline|], get_P: [||], set_Item: [|Inline|]"

    check "vwlnwer-0wreknj4" (test3()) "Equals: [||], GetHashCode: [||], GetType: [||], Object.get_ExtensionMethod: [|Inline|], Object.get_Item: [|Inline|], Object.set_Item: [|Inline|], ToString: [||]"


module ParamArrayNullAttribute = 
    open System

    type Attr([<ParamArray>] pms: obj[]) =
      inherit Attribute()
      override x.ToString() = sprintf "Attr(%A)" pms

    [<Attr(null)>]
    let f () = ()

    let test3() = 
        match <@ f() @> with
        | Quotations.Patterns.Call(_, m, _) ->
            m.GetCustomAttributes(typeof<Attr>, false)
            |> Seq.map (fun x -> x.ToString())
            |> String.concat ", "
        | _ -> failwith "unreachable 3"

    check "vwcewecioj9" (test3()) "Attr(<null>)"
 

// See https://github.com/fsharp/fsharp/issues/483
// We do not expect an exception
module TestFsiLoadOfNonExistentAssembly = 
    let test() = 
      try 
        let log4netType = System.Type.GetType("ThisTypeDoes.Not.Exist, thisAssemblyDoesNotExist")
        let exists = log4netType <> null
        if exists then report_failure (sprintf "type existed!")
        do printfn "%A" exists
       with e -> 
         report_failure (sprintf "exception unexpected: %s" e.Message)

    do test()

// See https://github.com/Microsoft/visualfsharp/issues/681
module BugWithOverloadedAttributes = 

    type FooAttribute(value : int) =
        inherit System.Attribute()
        new () = new FooAttribute(-1)

    [<FooAttribute(value = 42)>]
    type Bar = class end

#if !TESTS_AS_APP && !NETCOREAPP
module Bug719b = 

    open TestLibModule.Bug719
    
    type Bar =
        interface IFoo with
            member __.Test (?value:int) = value.ToString()
#endif

(*-------------------------------------------------------------------------
!* Test passed?
 *------------------------------------------------------------------------- *)


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

