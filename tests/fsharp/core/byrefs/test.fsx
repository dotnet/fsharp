// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_byrefs
#endif


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

[<Struct>]
type S = 
    [<DefaultValue(true)>]
    val mutable X : int 

#if NEGATIVE
module ByrefNegativeTests =

    module WriteToInRef = 
        let f1 (x: inref<int>) = x <- 1 // not allowed

    module WriteToInRefStructInner = 
        let f1 (x: inref<S>) = x.X <- 1 //not allowed

    module InRefToByRef = 
        let f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = f1 &x    // not allowed 

    module InRefToOutRef = 
        let f1 (x: outref<'T>) = 1
        let f2 (x: inref<'T>) = f1 &x     // not allowed

    module InRefToByRefClassMethod = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1 &x // not allowed

    module InRefToOutRefClassMethod =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed (not yet)
        let f2 (x: inref<'T>) = C.f1 &x // not allowed

    module InRefToByRefClassMethod2 = 
        type C() = 
            static member f1 (x: byref<'T>) = 1
        let f2 (x: inref<'T>) = C.f1(&x) // not allowed

    module InRefToOutRefClassMethod2 =
        type C() = 
            static member f1 (x: outref<'T>) = 1 // not allowed (not yet)
        let f2 (x: inref<'T>) = C.f1(&x) // not allowed

    module UseOfLibraryOnly =
        type C() = 
            static member f1 (x: byref<'T, 'U>) = 1

    module CantTakeAddress =

        let test1 () =
            let x = &1 // not allowed
            let y = &2 // not allowed
            x + y

        let test2_helper (x: byref<int>) = x
        let test2 () =
            let mutable x = 1
            let y = &test2_helper &x // not allowed
            ()

    module InRefParam_DateTime = 
        type C() = 
            static member M(x: inref<System.DateTime>) = x
        let w = System.DateTime.Now
        let v =  C.M(w) // not allowed
        check "cweweoiwe51btw" v w
             
    type byref<'T> with

        member this.Test() = 1

    type inref<'T> with

        member this.Test() = 1

    type outref<'T> with

        member this.Test() = 1

    module CantTakeAddressOfExpressionReturningReferenceType =
        open System.Collections.Concurrent
        open System.Collections.Generic

        let test1 () =
            let aggregator = 
                new ConcurrentDictionary<
                        string, ConcurrentDictionary<string,array<float>>
                        >()

            for kvp in aggregator do
            for kvpInner in kvp.Value do
                kvp.Value.TryRemove(
                    kvpInner.Key,
                    &kvpInner.Value)
                |> ignore

        let test2 () =
            let x = KeyValuePair(1, [||])
            let y = &x.Value
            ()
#endif

// Test a simple ref  argument
module CompareExchangeTests = 
    let mutable x = 3
    let v =  System.Threading.Interlocked.CompareExchange(&x, 4, 3)
    check "cweweoiwekla" v 3
    let v2 =  System.Threading.Interlocked.CompareExchange(&x, 5, 3)
    check "cweweoiweklb" v2 4

// Test a simple out argument
module TryGetValueTests = 
    let d = dict [ (3,4) ]
    let mutable res = 9
    let v =  d.TryGetValue(3, &res)
    check "cweweoiwekl1" v true
    check "cweweoiwekl2" res 4
    let v2 =  d.TryGetValue(5, &res)
    check "cweweoiwekl3" v2 false
    check "cweweoiwekl4" res 4


module ByRefParam  = 
    type C() = 
         static member M(x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker1" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreeker2" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker3" (minfo.ReturnParameter.IsIn) false
    check "cwnoreeker4" (minfo.ReturnParameter.IsOut) false

module ByRefParam_ExplicitOutAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker5" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreeker6a" (minfo.GetParameters().[0].IsOut) true
    check "cwnoreeker6b" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekers1" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreeker7" (minfo.ReturnParameter.IsIn) false
    check "cwnoreeker8" (minfo.ReturnParameter.IsOut) false

module ByRefParam_ExplicitInAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker9" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekerq" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6c" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekers2" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreekerw" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekere" (minfo.ReturnParameter.IsOut) false

module ByRefReturn  = 
    type C() = 
         static member M(x: byref<int>) = x <- x + 1; &x
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwvw4" v 10

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker6d" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekerr" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekert" (minfo.ReturnParameter.IsOut) false


module Slot_ByRefReturn  = 
    type I = 
         abstract M : x: byref<int> -> byref<int>
    type C() = 
         interface I with 
             member __.M(x: byref<int>) = x <- 5; &x
    let mutable res = 9
    let v =  (C() :> I).M(&res)
    check "cweweoiwek28989" res 5
    check "cweweoiwek28989" v 5

    let minfo = typeof<I>.GetMethod("M")
    check "cwnoreeker6e" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekery" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreekeru" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreekeri" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekers" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreekero" (minfo.ReturnParameter.IsOut) false

module InRefReturn  = 
    type C() = 
         static member M(x: inref<int>) = &x
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwvw4" v 9

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreekerp" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekera" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6f" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0 // modreq only placed on abstract/virtual
    check "cwnoreekers3" (minfo.ReturnParameter.IsIn) false // has modreq 'In' but reflection never returns true for ReturnParameter.IsIn
    check "cwnoreekers4" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 1
    check "cwnoreekerd" (minfo.ReturnParameter.IsOut) false

module Slot_InRefReturn  = 
    type I = 
         abstract M : x: inref<int> -> inref<int>
    type C() = 
         interface I with 
             member __.M(x: inref<int>) = &x
    let mutable res = 9
    let v =  (C() :> I).M(&res)
    check "cweweoiwek28989" res 9
    check "cweweoiwek28989" v 9

    let minfo = typeof<I>.GetMethod("M")
    check "cwnoreekerp" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekera" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6g" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 1
    check "cwnoreekers5" (minfo.ReturnParameter.IsIn) false // has modreq 'In' but reflection never returns true for ReturnParameter.IsIn
    check "cwnoreekers6" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 1
    check "cwnoreekerd" (minfo.ReturnParameter.IsOut) false


module OutRefParam_ExplicitOutAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 5
    let mutable res = 9
    C.M(&res)
    check "cweweoiweklceew4" res 5

module OutRefParam  = 
    type C() = 
         static member M(x: outref<int>) = x <- 5
    let mutable res = 9
    C.M(&res)
    check "cweweoiwek28989" res 5

module Slot_OutRefParam  = 
    type I = 
         abstract M : x: outref<int> -> unit
    type C() = 
         interface I with 
             member __.M(x: outref<int>) = x <- 5
    let mutable res = 9
    (C() :> I).M(&res)
    check "cweweoiwek28989" res 5

module ByRefParam_OverloadedTest_ExplicitOutAttribute  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 7
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2cbe9" res 8
    C.M(3, &res)
    check "cweweoiwek28498" res 7

module OutRefParam_Overloaded_ExplicitOutAttribute   = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 7
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2v90" res 8
    C.M(3, &res)
    check "cweweoiwek2c98" res 7

module OutRefParam_Overloaded  = 
    type C() = 
         static member M(a: int, x: outref<int>) = x <- 7
         static member M(a: string, x: outref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2v99323" res 8
    C.M(3, &res)
    check "cweweoiwe519" res 7

module InRefParam_ExplicitInAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<int>) = ()
    let mutable res = 9
    C.M(&res)
    check "cweweoiwe519btr" res 9

module InRefParam_ExplicitInAttributeDateTime = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<System.DateTime>) = x
    let Test() = 
        let res = System.DateTime.Now
        let v = C.M(&res)
        check "cweweoiwe519cw" v res
    Test()

module InRefParam  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let Test() = 
        let res = System.DateTime.Now
        let v =  C.M(&res)
        check "cweweoiwe51btw" v res

        let minfo = typeof<C>.GetMethod("M")
        check "cwnoreekerf" (minfo.GetParameters().[0].IsIn) true
        check "cwnoreekerg" (minfo.GetParameters().[0].IsOut) false
    Test()

module InRefParamOverload_ExplicitAddressOfAtCallSite  = 
    type C() = 
         static member M(x: System.DateTime) = x.AddDays(1.0)
         static member M(x: inref<System.DateTime>) = x.AddDays(2.0)
         static member M2(x: System.DateTime, y: int) = x.AddDays(1.0)
         static member M2(x: inref<System.DateTime>, y: int) = x.AddDays(2.0)

    let Test() = 
        let res = System.DateTime.Now
        let v =  C.M(&res)
        check "cweweoiwe51btw8" v (res.AddDays(2.0))
        let v2 =  C.M2(&res, 0)
        check "cweweoiwe51btw6" v2 (res.AddDays(2.0))

    Test()

module InRefParamOverload_ImplicitAddressOfAtCallSite  = 
    type C() = 
         static member M(x: System.DateTime) = x.AddDays(1.0)
         static member M(x: inref<System.DateTime>) = x.AddDays(2.0)
         static member M2(x: System.DateTime, y: int) = x.AddDays(1.0)
         static member M2(x: inref<System.DateTime>, y: int) = x.AddDays(2.0)
    let res = System.DateTime.Now
    let v =  C.M(res)
    check "cweweoiwe51btw1" v (res.AddDays(1.0))
    let v2 =  C.M2(res, 4)
    check "cweweoiwe51btw2" v2 (res.AddDays(1.0))


module InRefParamOverload_ImplicitAddressOfAtCallSite2  = 
    type C() = 
         static member M(x: System.DateTime) = x.AddDays(1.0)
         static member M(x: inref<System.DateTime>) = x.AddDays(2.0)
         static member M2(x: System.DateTime, y: int) = x.AddDays(1.0)
         static member M2(x: inref<System.DateTime>, y: int) = x.AddDays(2.0)
    let Test() = 
        let res = System.DateTime.Now
        let v =  C.M(res)
        check "cweweoiwe51btw1" v (res.AddDays(1.0))
        let v2 =  C.M2(res, 4)
        check "cweweoiwe51btw2" v2 (res.AddDays(1.0))
    Test()

#if IMPLICIT_ADDRESS_OF
module InRefParam_DateTime   = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let w = System.DateTime.Now
    let v =  C.M(w)
    check "cweweoiwe51btw" v w

module InRefParam_DateTime_ImplicitAddressOfAtCallSite  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let v =  C.M(System.DateTime.Now)
    check "cweweoiwe51btw" v.Date System.DateTime.Now.Date

module InRefParam_DateTime_ImplicitAddressOfAtCallSite2   = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let v =  C.M(System.DateTime.Now.AddDays(1.0))
    check "cweweoiwe51btw" v.Date (System.DateTime.Now.AddDays(1.0).Date)

module InRefParam_DateTime_ImplicitAddressOfAtCallSite3  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let mutable w = System.DateTime.Now
    let v =  C.M(w)
    check "cweweoiwe51btw" v w

module InRefParam_DateTime_ImplicitAddressOfAtCallSite4  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let date = System.DateTime.Now.Date
    let w = [| date |]
    let v =  C.M(w.[0])
    check "lmvjvwo1" v date
#endif

module InRefParam_Generic_ExplicitAddressOfAttCallSite1 = 
    type C() = 
         static member M(x: inref<'T>) = x
    let Test() = 
        let res = "abc"
        let v =  C.M(&res)
        check "lmvjvwo2" res "abc"
        check "lmvjvwo3" v "abc"
    Test()

module InRefParam_Generic_ExplicitAddressOfAttCallSite2  = 
    type C() = 
         static member M(x: inref<'T>) = x
    let Test() = 
        let res = "abc"
        let v =  C.M(&res)
        check "lmvjvwo4" v "abc"
    Test()

module ByrefReturnTests = 

    module TestImmediateReturn =
        let mutable x = 1

        let f () = &x

        let test() = 
            let addr : byref<int> = &f()
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
            let addr = &f 3
            addr <- addr + 1
            check2 "cepojcwem2" 2 x
            check2 "cepojcwem3" 1 y
            let addr = &f 4
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
            let addr = &f 3
            addr <- addr + 1
            check2 "cepojcwem6" 2 x
            check2 "cepojcwem7" 1 y
            let addr = &f 4
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

    module TestTryWithReturn =
        let mutable x = 1
        let mutable y = 1

        let f inp = try &x with _ -> &y

        let test() = 
            let addr = &f 3
            addr <- addr + 1
            check2 "cepojcwem6b" 2 x
            check2 "cepojcwem7b" 1 y
            let addr = &f 4
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
            let addr = &f 3
            addr <- addr + 1
            check2 "cepojcwem6b" 2 x
            check2 "cepojcwem7b" 1 y
            let addr = &f 4
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
            let addr = &f &r1
            addr <- addr + 1
            check2 "cepojcwem10" 2 r1

        test()

    module TestTwoArguments =

        let f (x:byref<int>, y:byref<int>) = &x

        let test() = 
            let mutable r1 = 1
            let mutable r2 = 0
            let addr = &f (&r1, &r2)
            addr <- addr + 1
            check2 "cepojcwem11" 2 r1

        test()

    module TestRecordParam =

        type R = { mutable z : int }
        let f (x:R) = &x.z

        let test() = 
            let r = { z = 1 }
            let addr = &f r
            addr <- addr + 1
            check2 "cepojcwem12" 2 r.z

        test()

    module TestRecordParam2 =

        type R = { mutable z : int }
        let f (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = &f &r
            addr <- addr + 1
            check2 "cepojcwem13a" 2 r.z

        test()

    module TestClassParamMutableField =

        type C() = [<DefaultValue>] val mutable z : int

        let f (x:C) = &x.z

        let test() = 
            let c = C()
            let addr = &f c
            addr <- addr + 1
            check2 "cepojcwem13b" 1 c.z 

        test()

    module TestArrayParam =

        let f (x:int[]) = &x.[0]

        let test() = 
            let r = [| 1 |]
            let addr = &f r
            addr <- addr + 1
            check2 "cepojcwem14" 2 r.[0]

        test()

    module TestStructParam =

        [<Struct>]
        type R = { mutable z : int }

        let f (x:byref<R>) = &x.z

        let test() = 
            let mutable r = { z = 1 }
            let addr = &f &r
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
            let addr = &f (C()) 
            addr <- addr + 1
            let addr = &f (ObjExpr()) 
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
            let addr = &f (C()) 
            addr <- addr + 1
            let addr = &f (ObjExpr()) 
            addr <- addr + 1
            check2 "cepojcwem17" 3 x

        test()

    module TestDelegateMethod =
        let mutable x = 1

        type D = delegate of unit ->  byref<int>

        let d() = D(fun () -> &x)

        let f (d:D) = &d.Invoke()

        let test() = 
            let addr = &f (d()) 
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
            let addr = &f (d()) 
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

    module TestTryWithReturn =
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


    module ByRefExtensionMethods1 = 

        open System
        open System.Runtime.CompilerServices

        [<Extension>]
        type Ext = 
        
            [<Extension>]
            static member ExtDateTime2(dt: inref<DateTime>, x:int) = dt.AddDays(double x)
        
        module UseExt = 
            let now = DateTime.Now
            let dt2 = now.ExtDateTime2(3)
            check "£f3mllkm2" dt2 (now.AddDays(3.0))
            

(*
    module ByRefExtensionMethodsOverloading = 

        open System
        open System.Runtime.CompilerServices

        [<Extension>]
        type Ext = 
            [<Extension>]
            static member ExtDateTime(dt: DateTime, x:int) = dt.AddDays(double x)
        
            [<Extension>]
            static member ExtDateTime(dt: inref<DateTime>, x:int) = dt.AddDays(2.0 * double x)
        
        module UseExt = 
            let dt = DateTime.Now.ExtDateTime(3)
            let dt2 = DateTime.Now.ExtDateTime(3)
*)
    module TestReadOnlyAddressOfStaticField = 
        type C() =
           static let x = 1
           static member F() = &x

        let test() =
            let addr = &C.F()
            check2 "mepojcwem18a2dw" 1 addr

        test()

    module TestAssignToReturnByref = 
        type C() = 
            static let mutable v = System.DateTime.Now
            static member M() = &v
            static member P = &v
            member __.InstanceM() = &v
            member __.InstanceP with get() = &v
            static member Value = v

        let F1() = 
            let today = System.DateTime.Now.Date
            C.M() <-  today
            check "cwecjc" C.Value  today
            C.P <- C.M().AddDays(1.0)
            check "cwecjc1" C.Value (today.AddDays(1.0))
            let c = C() 
            c.InstanceM() <-  today.AddDays(2.0)
            check "cwecjc2" C.Value (today.AddDays(2.0))
            c.InstanceP <-  today.AddDays(3.0)
            check "cwecjc1" C.Value (today.AddDays(3.0))

        F1()

    module TestAssignToReturnByref2 = 
        let mutable v = System.DateTime.Now
        let M() = &v

        let F1() = 
            let today = System.DateTime.Now.Date
            M() <-  today
            check "cwecjc" v  today

        F1()

    module BaseCallByref = 

        type Incrementor(z) =
            abstract member Increment : int byref * int byref -> unit
            default this.Increment(i : int byref,j : int byref) =
               i <- i + z

        type Decrementor(z) =
            inherit Incrementor(z)
            override this.Increment(i, j) =
                base.Increment(&i, &j)

                i <- i - z


    module Bug820 = 

        let inline f (x, r:byref<_>) = r <- x
        let mutable x = Unchecked.defaultof<_>
        f (0, &x)

    module Bug820b = 

        type  Bug820x() = 
            let f (x, r:byref<_>) = r <- x
            let mutable x = Unchecked.defaultof<_>
            member __.P = f (0, &x)

    // check recursive functions
    module TestNameModuleGeneric =

        let rec testValue (unused: 'T) id (data: byref<byte>) : unit =
            if id = 10 then 
                data <- 3uy 
            else
                 testValue unused (id + 1) &data
        let Test() = 
            let mutable x = 0uy
            testValue "unused" 0 &x
            check "vruoer" x 3uy
        Test()

    module TestNameModuleNonGeneric =

        let rec testValue id (data: byref<byte>) : unit =
            if id = 10 then 
                data <- 3uy 
            else 
                testValue (id + 1) &data

        let Test() = 
            let mutable x = 0uy
            testValue  0 &x
            check "vruoer3r" x 3uy
        Test()

    module TestNameModuleNonGenericSubsume =

        let rec testValue id (data: byref<byte>) (y: System.IComparable) : unit =
            if id = 10 then 
                data <- 3uy 
            else 
                testValue (id + 1) &data y

        let Test() = 
            let mutable x = 0uy
            testValue  0 &x Unchecked.defaultof<System.IComparable>
            check "vruoer3r" x 3uy 

        Test()

    type GenericTestNameRecursive() =

        let rec testValue unused id (data: byref<byte>) : unit =
            if id = 10 then data <- 3uy else testValue unused (id + 1) &data

        static do GenericTestNameRecursive().Test()

        member __.Test() = 
            let mutable x = 0uy
            testValue "unused" 0 &x
            check "vruoer3rv" x 3uy
            let mutable z = 0uy
            testValue 6L 0 &z
            check "vruoer3rvwqf" z 3uy

    type NonGenericTestNameRecursiveInClass() =

        let rec testValue id (data: byref<byte>) : unit =
            if id = 10 then 
                data <- 3uy 
            else 
                testValue (id + 1) &data

        static do NonGenericTestNameRecursiveInClass().Test()

        member __.Test() = 
            let mutable x = 0uy
            testValue  0 &x
            check "vruoer3rvvremtys" x 3uy


    type NonGenericTestNameRecursiveInClassSubsume() =

        let rec testValue id (data: byref<byte>) (y:System.IComparable) : unit =
            if id = 10 then 
                data <- 3uy 
            else 
                testValue (id + 1) &data y

        static do NonGenericTestNameRecursiveInClassSubsume().Test()

        member __.Test() = 
            let mutable x = 0uy
            testValue  0 &x Unchecked.defaultof<System.IComparable>
            check "vruoer3rvvremtys" x 3uy

    type StaticGenericTestNameRecursiveInClass() =

        static let rec testValue unused id (data: byref<byte>) : unit =
            if id = 10 then data <- 3uy else testValue unused (id + 1) &data

        static do StaticGenericTestNameRecursiveInClass.Test()

        static member Test() = 
            let mutable x = 0uy
            testValue "unused" 0 &x
            check "vruoer3rv" x 3uy
            let mutable z = 0uy
            testValue 6L 0 &z
            check "vruoer3rvwqfgw" z 3uy

    type StaticNonGenericTestNameRecursiveInClass() =

        static let rec testValue id (data: byref<byte>) : unit =
            if id = 10 then data <- 3uy else testValue (id + 1) &data

        static do StaticNonGenericTestNameRecursiveInClass.Test()

        static member Test() = 
            let mutable x = 0uy
            testValue  0 &x
            check "vruoer3rvvrebae" x 3uy

    module TestInRefMutation = 
        [<Struct>]
        type TestMut =

            val mutable x : int

            member this.AnAction() =
                this.x <- 1

        let testAction (m: inref<TestMut>) =
            m.AnAction()
            check "vleoij" m.x 0

        let test() =
            let x = TestMut()
            //testIn (&x)
            testAction (&x)
            x            
        test()

    module MutateInRef3 =
        [<Struct>]
        type TestMut(x: int ref) =

            member this.X = x.contents
            member this.XAddr = &x.contents

        let testIn (m: inref<TestMut>) =
            // If the struct API indirectly reveals a byref return of a field in a reference type then  
            // there is nothing stopping it being written to.
            m.XAddr <- 1

        let test() =
            let m = TestMut(ref 0)
            testIn (&m)
            check "vleoij" m.X 1

        test()

    module MatrixOfTests = 

        module ReturnAddressOfByRef = 
            let f1 (x: byref<int>) = &x 

        module ReturnAddressOfInRef = 
            let f1 (x: inref<int>) = &x 

        module ReturnAddressOfOutRef = 
            let f1 (x: outref<int>) = &x 

        //-----

        module ReadByRef = 
            let f1 (x: byref<int>) = x 

        module ReadInRef = 
            let f1 (x: inref<int>) = x 

        module ReadOutRef = 
            let f1 (x: outref<int>) = x 

        //-----

        module ReadByRefStructInner = 
            let f1 (x: byref<S>) = x.X

        module ReadInRefStructInner = 
            let f1 (x: inref<S>) = x.X

        module ReadOutRefStructInner = 
            let f1 (x: outref<S>) = x.X

        //-----
        module WriteToByRef = 
            let f1 (x: byref<int>) = x <- 1

        module WriteToOutRef = 
            let f1 (x: outref<int>) = x <- 1

        //-----
        module WriteToByRefStructInner = 
            let f1 (x: byref<S>) = x.X <- 1

        module WriteToOutRefStructInner = 
            let f1 (x: outref<S>) = x.X <- 1

        //-----
        module OutRefToByRef = 
            let f1 (x: byref<'T>) = 1
            let f2 (x: outref<'T>) = f1 &x 

        module ByRefToByRef = 
            let f1 (x: byref<'T>) = 1
            let f2 (x: byref<'T>) = f1 &x        

        module ByRefToOutRef = 
            let f1 (x: outref<'T>) = 1
            let f2 (x: byref<'T>) = f1 &x        

        module OutRefToOutRef = 
            let f1 (x: outref<'T>) = 1
            let f2 (x: outref<'T>) = f1 &x        

        module ByRefToInRef = 
            let f1 (x: inref<'T>) = 1
            let f2 (x: byref<'T>) = f1 &x        

        module InRefToInRef = 
            let f1 (x: inref<'T>) = 1
            let f2 (x: inref<'T>) = f1 &x        

        module OutRefToInRef = 
            let f1 (x: inref<'T>) = 1
            let f2 (x: outref<'T>) = f1 &x    // allowed, because &outref are treated as byref, see RFC

        //---------------
        module OutRefToByRefClassMethod = 
            type C() = 
                static member f1 (x: byref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1 &x

        module ByRefToByRefClassMethod =
            type C() = 
                static member f1 (x: byref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1 &x        

        module ByRefToOutRefClassMethod =
            type C() = 
                static member f1 (x: outref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1 &x        

        module OutRefToOutRefClassMethod =
            type C() = 
                static member f1 (x: outref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1 &x        

        module ByRefToInRefClassMethod =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1 &x        

        module InRefToInRefClassMethod =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: inref<'T>) = C.f1 &x        

        module OutRefToInRefClassMethod =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1 &x        

        //---------------
        module OutRefToByRefClassMethod2 = 
            type C() = 
                static member f1 (x: byref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1(&x)

        module ByRefToByRefClassMethod2 =
            type C() = 
                static member f1 (x: byref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1(&x)        

        module ByRefToOutRefClassMethod2 =
            type C() = 
                static member f1 (x: outref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1(&x)        

        module OutRefToOutRefClassMethod2 =
            type C() = 
                static member f1 (x: outref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1(&x)        

        module ByRefToInRefClassMethod2 =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: byref<'T>) = C.f1(&x)        

        module InRefToInRefClassMethod2 =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: inref<'T>) = C.f1(&x)        

        module OutRefToInRefClassMethod2 =
            type C() = 
                static member f1 (x: inref<'T>) = 1
            let f2 (x: outref<'T>) = C.f1(&x)

    module TestStructRecord =
        [<Struct>]
        type AnItem =
            { Link: string }

        let link item =
            { item with Link = "" }        

// https://github.com/dotnet/fsharp/issues/12085
module NoTailcallToByrefsWithModReq =
    module ClassLibrary =

        // F# equivalent of `public delegate FieldType Getter<DeclaringType, FieldType>(in DeclaringType);`
        type Getter<'DeclaringType, 'FieldType> = delegate of inref<'DeclaringType> -> 'FieldType 

        type GetterWrapper<'DeclaringType, 'FieldType> (getter : Getter<'DeclaringType, 'FieldType>) =
            member _.Get (instance : 'DeclaringType) = getter.Invoke &instance
    
    open ClassLibrary
    type MyRecord = { Value: int[] }
    let App() =
        
        let wrapper = new GetterWrapper<MyRecord, int[]>(fun (record: inref<MyRecord>) -> record.Value)

        let record = { Value = [| 42 |] }
        let value = wrapper.Get(record)
        value.GetEnumerator()
    App()

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

