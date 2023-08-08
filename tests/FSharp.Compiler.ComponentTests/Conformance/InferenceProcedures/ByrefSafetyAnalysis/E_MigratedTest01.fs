// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_byrefs
#endif

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
                        string, ConcurrentDictionary<string, float array>
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
