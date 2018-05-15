// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_apporder
#endif

#light
let failures = ref false
let report_failure (s) = 
  stderr.WriteLine ("NO: " + s); failures := true
let test s b = if b then () else report_failure(s) 

(* TEST SUITE FOR Int32 *)

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else report_failure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)

let check2 s expected actual = check s actual expected 

// Test a simple ref  argument
module CompareExchangeTests = 
    let mutable x = 3
    let v =  System.Threading.Interlocked.CompareExchange(&x, 3, 4)

#if NEG
module CompareExchangeTests_Negative1 = 
    let x = 3
    let v =  System.Threading.Interlocked.CompareExchange(&x, 3, 4)

module CompareExchangeTests_Negative2 = 
    let v =  System.Threading.Interlocked.CompareExchange(&3, 3, 4)
#endif

// Test a simple out argument
module TryGetValueTests = 
    let d = dict [ (3,4) ]
    let mutable res = 9
    let v =  d.TryGetValue(3, &res)

#if NEG
module TryGetValueTests_Negative1 = 
    let d = dict [ (3,4) ]
    let res = 9
    let v =  d.TryGetValue(3, &res)
#endif


module FSharpDeclaredOutParamTest  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let mutable res = 9
    let v =  C.M(&res)


module FSharpDeclaredOutParamTest2  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: outref<int>) = ()
    let mutable res = 9
    let v =  C.M(&res)

module FSharpDeclaredOutParamTest3  = 
    type C() = 
         static member M(x: outref<int>) = ()
    let mutable res = 9
    let v =  C.M(&res)

#if NEG
module FSharpDeclaredOutParamTest_Neagative1  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let  res = 9
    let v =  C.M(&res)
#endif

module FSharpDeclaredOverloadedOutParamTest  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let mutable res = 9
    let v =  C.M("a", &res)
    let v2 =  C.M(3, &res)

module FSharpDeclaredOverloadedOutParamTest2  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: outref<int>) = ()
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: outref<int>) = ()
    let mutable res = 9
    let v =  C.M("a", &res)
    let v2 =  C.M(3, &res)

module FSharpDeclaredOverloadedOutParamTest3  = 
    type C() = 
         static member M(a: int, x: outref<int>) = ()
         static member M(a: string, x: outref<int>) = ()
    let mutable res = 9
    let v =  C.M("a", &res)
    let v2 =  C.M(3, &res)

#if NEG
module FSharpDeclaredOverloadedOutParamTest_Negative1  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let  res = 9
    let v =  C.M("a", &res)
    let v2 =  C.M(3, &res)

module FSharpDeclaredOutParamTest_Negative1  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = ()
    let res = 9
    let v =  C.M(&res)
#endif

module FSharpDeclaredInParamTest  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<int>) = ()
    let mutable res = 9
    let v =  C.M(&res)

module FSharpDeclaredInParamTest2  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<System.DateTime>) = ()
    let res = System.DateTime.Now
    let v =  C.M(&res)

module FSharpDeclaredInParamTest3  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = ()
    let res = System.DateTime.Now
    let v =  C.M(&res)

module FSharpDeclaredInParamTest4  = 
    type C() = 
         static member M(x: inref<'T>) = ()
    let res = "abc"
    let v =  C.M(&res)

module FSharpDeclaredInParamTest5  = 
    type C() = 
         static member M(x: inref<'T>) = ()
    let res = "abc"
    let v =  C.M(&res)

module ByrefReturnTests = 

    module TestImmediateReturn =
        let mutable x = 1

        let f () = &x

        let test() = 
            let addr : byref<int> = f()
            addr <- addr + 1
            check2 "cepojcwem1" 2 x


        let test2() = 
            let v = f()
            let res = v + 1
            check2 "cepojcwem1b" 3 res

        test()
        test2()

    module TestMatchReturn =
        let mutable x = 1
        let mutable y = 1

        let f inp = match inp with 3 -> &x | _ -> &y

        let test() = 
            let addr = f 3
            addr <- addr + 1
            check2 "cepojcwem2" 2 x
            check2 "cepojcwem3" 1 y
            let addr = f 4
            addr <- addr + 1
            check2 "cepojcwem4" 2 x
            check2 "cepojcwem5" 2 y

        let test2() = 
            let res = f 3
            let res2 = res + 1
            check2 "cepojcwem2b" 3 res2
            check2 "cepojcwem3b" 2 res

        test()
        test2()

    module TestConditionalReturn =
        let mutable x = 1
        let mutable y = 1

        let f inp = if inp = 3 then &x else &y

        let test() = 
            let addr = f 3
            addr <- addr + 1
            check2 "cepojcwem6" 2 x
            check2 "cepojcwem7" 1 y
            let addr = f 4
            addr <- addr + 1
            check2 "cepojcwem8" 2 x
            check2 "cepojcwem9" 2 y

        let test2() = 
            let res = f 3
            let res2 = res + 1
            check2 "cepojcwem8b" 3 res2
            check2 "cepojcwem9b" 2 res

        test()
        test2()

    module TestTryCatchReturn =
        let mutable x = 1
        let mutable y = 1

        let f inp = try &x with _ -> &y

        let test() = 
            let addr = f 3
            addr <- addr + 1
            check2 "cepojcwem6b" 2 x
            check2 "cepojcwem7b" 1 y
            let addr = f 4
            addr <- addr + 1
            check2 "cepojcwem8b" 3 x
            check2 "cepojcwem9b" 1 y

        let test2() = 
            let res = f 3
            let res2 = res + 1
            check2 "cepojcwem2ff" 4 res2
            check2 "cepojcwem3gg" 3 res

        test()
        test2()

    module TestTryFinallyReturn =
        let mutable x = 1
        let mutable y = 1

        let f inp = try &x with _ -> &y

        let test() = 
            let addr = f 3
            addr <- addr + 1
            check2 "cepojcwem6b" 2 x
            check2 "cepojcwem7b" 1 y
            let addr = f 4
            addr <- addr + 1
            check2 "cepojcwem8b" 3 x
            check2 "cepojcwem9b" 1 y

        let test2() = 
            let res = f 3
            let res2 = res + 1
            check2 "cepojcwem2tf" 4 res2
            check2 "cepojcwem3qw" 3 res

        test()
        test2()

    module TestOneArgument =

        let f (x:byref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let addr = f &r1
            addr <- addr + 1
            check2 "cepojcwem10" 2 r1

        test()

    module TestTwoArguments =

        let f (x:byref<int>, y:byref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let mutable r2 = 0
            let addr = f (&r1, &r2)
            addr <- addr + 1
            check2 "cepojcwem11" 2 r1

        test()

    module TestRecordParam =

        type R = { mutable z : int }
        let f (x:R) = &x.z

        let test() = 
            let r = { z = 1 }
            let addr = f r
            addr <- addr + 1
            check2 "cepojcwem12" 2 r.z

        test()

    module TestRecordParam2 =

        type R = { mutable z : int }
        let f (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = f &r
            addr <- addr + 1
            check2 "cepojcwem13a" 2 r.z

        test()

    module TestClassParamMutableField =

        type C() = [<DefaultValue>] val mutable z : int

        let f (x:C) = &x.z

        let test() = 
            let c = C()
            let addr = f c
            addr <- addr + 1
            check2 "cepojcwem13b" 1 c.z 

        test()

    module TestArrayParam =

        let f (x:int[]) = &x.[0]

        let test() = 
            let r = [| 1 |]
            let addr = f r
            addr <- addr + 1
            check2 "cepojcwem14" 2 r.[0]

        test()

    module TestStructParam =

        [<Struct>]
        type R = { mutable z : int }

        let f (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = f &r
            addr <- addr + 1
            check2 "cepojcwem15" 2 r.z

        test()

    module TestInterfaceMethod =
        let mutable x = 1

        type I = 
            abstract M : unit -> byref<int>

        type C() = 
            interface I with 
                member this.M() = &x

        let ObjExpr() = 
            { new I with 
                member this.M() = &x }

        let f (i:I) = &i.M()

        let test() = 
            let addr = f (C()) 
            addr <- addr + 1
            let addr = f (ObjExpr()) 
            addr <- addr + 1
            check2 "cepojcwem16" 3 x

        test()

    module TestInterfaceProperty =
        let mutable x = 1

        type I = 
            abstract P : byref<int>

        type C() = 
            interface I with 
                member this.P = &x

        let ObjExpr() = 
            { new I with 
                member this.P = &x }

        let f (i:I) = &i.P

        let test() = 
            let addr = f (C()) 
            addr <- addr + 1
            let addr = f (ObjExpr()) 
            addr <- addr + 1
            check2 "cepojcwem17" 3 x

        test()

    module TestDelegateMethod =
        let mutable x = 1

        type D = delegate of unit ->  byref<int>

        let d() = D(fun () -> &x)

        let f (d:D) = &d.Invoke()

        let test() = 
            let addr = f (d()) 
            check2 "cepojcwem18a" 1 x
            addr <- addr + 1
            check2 "cepojcwem18b" 2 x

        test()

    module TestBaseCall =
        type Incrementor(z) =
            abstract member Increment : int byref * int byref -> unit
            default this.Increment(i : int byref,j : int byref) =
               i <- i + z

        type Decrementor(z) =
            inherit Incrementor(z)
            override this.Increment(i, j) =
                base.Increment(&i, &j)

                i <- i - z

    module TestDelegateMethod2 =
        let mutable x = 1

        type D = delegate of byref<int> ->  byref<int>

        let d() = D(fun xb -> &xb)

        let f (d:D) = &d.Invoke(&x)

        let test() = 
            let addr = f (d()) 
            check2 "cepojcwem18a2" 1 x
            addr <- addr + 1
            check2 "cepojcwem18b3" 2 x

        test()

module ByrefReturnMemberTests = 

    module TestImmediateReturn =
        let mutable x = 1

        type C() = 
            static member M () = &x

        let test() = 
            let addr : byref<int> = &C.M()
            addr <- addr + 1
            check2 "mepojcwem1" 2 x


        let test2() = 
            let v = &C.M()
            let res = v + 1
            check2 "mepojcwem1b" 3 res

        test()
        test2()

    module TestMatchReturn =
        let mutable x = 1
        let mutable y = 1

        type C() = 
            static member M inp = match inp with 3 -> &x | _ -> &y

        let test() = 
            let addr = &C.M 3
            addr <- addr + 1
            check2 "mepojcwem2" 2 x
            check2 "mepojcwem3" 1 y
            let addr = &C.M 4
            addr <- addr + 1
            check2 "mepojcwem4" 2 x
            check2 "mepojcwem5" 2 y

        let test2() = 
            let res = &C.M 3
            let res2 = res + 1
            check2 "mepojcwem2b" 3 res2
            check2 "mepojcwem3b" 2 res

        test()
        test2()

    module TestConditionalReturn =
        let mutable x = 1
        let mutable y = 1

        type C() = 
            static member M inp = if inp = 3 then &x else &y

        let test() = 
            let addr = &C.M 3
            addr <- addr + 1
            check2 "mepojcwem6" 2 x
            check2 "mepojcwem7" 1 y
            let addr = &C.M 4
            addr <- addr + 1
            check2 "mepojcwem8" 2 x
            check2 "mepojcwem9" 2 y

        let test2() = 
            let res = &C.M 3
            let res2 = res + 1
            check2 "mepojcwem8b" 3 res2
            check2 "mepojcwem9b" 2 res

        test()
        test2()

    module TestTryCatchReturn =
        let mutable x = 1
        let mutable y = 1

        type C() = 
            static member M inp = try &x with _ -> &y

        let test() = 
            let addr = &C.M 3
            addr <- addr + 1
            check2 "mepojcwem6b" 2 x
            check2 "mepojcwem7b" 1 y
            let addr = &C.M 4
            addr <- addr + 1
            check2 "mepojcwem8b" 3 x
            check2 "mepojcwem9b" 1 y

        let test2() = 
            let res = &C.M 3
            let res2 = res + 1
            check2 "mepojcwem2ff" 4 res2
            check2 "mepojcwem3gg" 3 res

        test()
        test2()

    module TestTryFinallyReturn =
        let mutable x = 1
        let mutable y = 1

        type C() = 
            static member M inp = try &x with _ -> &y

        let test() = 
            let addr = &C.M 3
            addr <- addr + 1
            check2 "mepojcwem6b" 2 x
            check2 "mepojcwem7b" 1 y
            let addr = &C.M 4
            addr <- addr + 1
            check2 "mepojcwem8b" 3 x
            check2 "mepojcwem9b" 1 y

        let test2() = 
            let res = &C.M 3
            let res2 = res + 1
            check2 "mepojcwem2tf" 4 res2
            check2 "mepojcwem3qw" 3 res

        test()
        test2()

    module TestOneArgument =

        type C() = 
            static member M (x:byref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let addr = &C.M (&r1)
            addr <- addr + 1
            check2 "mepojcwem10" 2 r1

        test()

#if NEG
    module TestOneArgumentInRefThenMutate_Negative1 =

        type C() = 
            static member M (x:inref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let addr = &C.M (&r1)
            addr <- addr + 1 // "The byref pointer is readonly, so this write is not permitted"
            check2 "mepojcwem10" 2 r1

        test()
#endif

    module TestOneArgumentInRefReturned =

        type C() = 
            static member M (x:inref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let addr = &C.M (&r1)
            let x = addr + 1
            check2 "mepojcwem10" 1 r1
            check2 "mepojcwem10vr" 2 x

        test()

    module TestOneArgumentOutRef =

        type C() = 
            static member M (x:outref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let addr = &C.M (&r1)
            addr <- addr + 1
            check2 "mepojcwem10" 2 r1

        test()

    module TestTwoArguments =

        type C() = 
            static member M (x:byref<int>, y:byref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let mutable r2 = 0
            let addr = &C.M (&r1, &r2)
            addr <- addr + 1
            check2 "mepojcwem11" 2 r1

        test()

    module TestRecordParam =

        type R = { mutable z : int }
        type C() = 
            static member M (x:R) = &x.z

        let test() = 
            let r = { z = 1 }
            let addr = &C.M r
            addr <- addr + 1
            check2 "mepojcwem12" 2 r.z

        test()

    module TestRecordParam2 =

        type R = { mutable z : int }
        type C() = 
            static member M (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = &C.M(&r)
            addr <- addr + 1
            check2 "mepojcwem13a" 2 r.z

        test()

    module TestClassParamMutableField =

        type C() = [<DefaultValue>] val mutable z : int

        type C2() = 
            static member M (x:C) = &x.z

        let test() = 
            let c = C()
            let addr = &C2.M c
            addr <- addr + 1
            check2 "mepojcwem13b" 1 c.z 

        test()

    module TestArrayParam =

        type C() = 
            static member M (x:int[]) = &x.[0]

        let test() = 
            let r = [| 1 |]
            let addr = &C.M r
            addr <- addr + 1
            check2 "mepojcwem14" 2 r.[0]

        test()

    module TestStructParam =

        [<Struct>]
        type R = { mutable z : int }

        type C() = 
            static member M (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = &C.M(&r)
            addr <- addr + 1
            check2 "mepojcwem15" 2 r.z

        test()

    module TestInterfaceMethod =
        let mutable x = 1

        type I = 
            abstract M : unit -> byref<int>

        type C() = 
            interface I with 
                member this.M() = &x

        let ObjExpr() = 
            { new I with 
                member this.M() = &x }

        let test() = 
            let addr = &(C() :> I).M()
            addr <- addr + 1
            let addr = &(ObjExpr()).M()
            addr <- addr + 1
            check2 "mepojcwem16" 3 x

        test()

    module TestInterfaceProperty =
        let mutable x = 1

        type I = 
            abstract P : byref<int>

        type C() = 
            interface I with 
                member this.P = &x

        let ObjExpr() = 
            { new I with 
                member this.P = &x }

        let test() = 
            let addr = &(C() :> I).P
            addr <- addr + 1
            let addr = &(ObjExpr()).P
            addr <- addr + 1
            check2 "mepojcwem17" 3 x

        test()

    module TestDelegateMethod =
        let mutable x = 1

        type D = delegate of unit ->  byref<int>

        let test() = 
            let d = D(fun () -> &x)
            let addr = &d.Invoke()
            check2 "mepojcwem18a" 1 x
            addr <- addr + 1
            check2 "mepojcwem18b" 2 x

        test()

    module TestBaseCall =
        type Incrementor(z) =
            abstract member Increment : int byref * int byref -> unit
            default this.Increment(i : int byref,j : int byref) =
               i <- i + z

        type Decrementor(z) =
            inherit Incrementor(z)
            override this.Increment(i, j) =
                base.Increment(&i, &j)

                i <- i - z

    module TestDelegateMethod2 =
        let mutable x = 1

        type D = delegate of byref<int> ->  byref<int>

        let d = D(fun xb -> &xb)

        let test() = 
            let addr = &d.Invoke(&x)
            check2 "mepojcwem18a2" 1 x
            addr <- addr + 1
            check2 "mepojcwem18b3" 2 x

        test()


let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

