open System.Runtime.InteropServices


module Test1 =
    type C1() =
        static member X(p: C1 byref) = p

    let inline callX<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = (^T: (static member X : 'T byref -> 'T) (&x))

    let mutable c1 = C1()
    let g1 = callX<C1> &c1

module Test2 =
    type C2() =
        static member X(p: C2 byref) = p

    let inline callX2<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = 'T.X &x
    let mutable c2 = C2()
    let g2 = callX2<C2> &c2

module Test3 =

    type C3() =
        static member X(p: C3 byref, n: int) = p

    let inline callX3<'T when 'T : (static member X: 'T byref * int -> 'T)> (x: 'T byref) = 'T.X (&x, 3)
    let mutable c3 = C3()
    let g3 = callX3<C3> &c3

module Test4 =
    type C4() =
        static member X() = C4()

    let inline callX4<'T when 'T : (static member X: unit -> 'T)> ()  = 'T.X ()
    let g4 = callX4<C4> ()

#if NEGATIVE
// NOTE, we don't expect these to compile.  Trait constraints taht involve byref returns
// currently can never be satisfied by any method.  No other warning is given - we may enable
// this at some later point but it is orthogonal to the RFC
module Test5 =
    type C5() =
        static member X(p: C5 byref) = &p

    let inline callX5<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = 'T.X &x
    let mutable c5 = C5()
    let g5 () = callX5<C5> &c5

module Test6 =

    type C6() =
        static member X(p: C6 byref) = &p

    // NOTE: you can declare trait call which returns the address of the thing provided, you just can't satisfy the constraint
    let inline callX6<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = &'T.X &x
    let mutable c6 = C6()
    let g6 () = callX6<C6> &c6

// No out args allows
module Test7 =

    let inline callX2<'T when 'T : (static member X: [<Out>] 'T byref -> bool)> () = ()
#endif
