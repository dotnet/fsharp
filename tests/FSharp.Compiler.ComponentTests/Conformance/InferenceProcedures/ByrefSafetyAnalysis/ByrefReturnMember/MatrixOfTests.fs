open Prelude

[<Struct>]
type S = 
    [<DefaultValue(true)>]
    val mutable X : int

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