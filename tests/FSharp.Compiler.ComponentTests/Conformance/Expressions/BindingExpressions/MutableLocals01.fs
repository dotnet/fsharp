namespace SomethingPithy

// #in #BindingExpressions
//
// This provides conformance testing for capture of local mutable values in closures
//<Expects status="warning" span="(34,21-34,32)" id="FS3180">The mutable local 'exec_test_1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed. </Expects>
//<Expects status="warning" span="(71,21-71,24)" id="FS3180">The mutable local 'ix1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(104,21-104,25)" id="FS3180">The mutable local 'ix1d' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(142,21-142,24)" id="FS3180">The mutable local 'ix2' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(149,21-149,24)" id="FS3180">The mutable local 'ix3' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(155,21-155,24)" id="FS3180">The mutable local 'ix4' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(185,21-185,24)" id="FS3180">The mutable local 'sx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(189,21-189,24)" id="FS3180">The mutable local 'sx2' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(196,21-196,24)" id="FS3180">The mutable local 'sx3' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(206,21-206,25)" id="FS3180">The mutable local 'nlx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(212,26-212,30)" id="FS3180">The mutable local 'nlx2' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(219,27-219,31)" id="FS3180">The mutable local 'nlx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(231,27-231,31)" id="FS3180">The mutable local 'nlx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(238,31-238,35)" id="FS3180">The mutable local 'nlx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(247,21-247,25)" id="FS3180">The mutable local 'seq3' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(254,29-254,33)" id="FS3180">The mutable local 'nlx1' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(285,22-285,23)" id="FS3180">The mutable local 'x' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(308,22-308,23)" id="FS3180">The mutable local 'x' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(322,22-322,23)" id="FS3180">The mutable local 'x' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>
//<Expects status="warning" span="(333,22-333,23)" id="FS3180">The mutable local 'x' is implicitly allocated as a reference cell because it has been captured by a closure. This warning is for informational purposes only to indicate where implicit allocations are performed.</Expects>

module ExecTests =

    let test1() =
        let mutable exec_test_1 = 1
        (fun () -> exec_test_1 <- exec_test_1 + 1), (fun () -> exec_test_1 <- exec_test_1 + 2), (fun () -> exec_test_1) 

    let incr, incr2, get = test1()


    let v1 = (get())
    let v2 = (get())
    incr()
    let v3 = (get())
    incr()
    incr()
    let v4 = (get())
    incr2()
    incr2()
    let v5 = (get())

    let exp = (1,1,2,4,8)

    let result () = (v1,v2,v3,v4,v5) = exp

    let check() = 
        if result() then true 
        else
            eprintfn "FAIL: (v1,v2,v3,v4,v5) = %A, expected %A" (v1,v2,v3,v4,v5) exp
            false


module IntTests =

    let test0() =

        let ix0_NO = 1
        [ (fun () -> ix0_NO + 1) ]

    let test1() =

        let mutable ix1 = 1
        [ (fun () -> ix1 <- ix1 + 1) ]

    let test1z() =

        let mutable ix1z_NOT_IF_OPT = 1
        let _ = (fun () -> ix1z_NOT_IF_OPT <- ix1z_NOT_IF_OPT + 1)  // throw it away
        ix1z_NOT_IF_OPT

    let test1b() =

        let mutable ix1b_NOT_IF_OPT = 1
        let localFunctionThatIsNotGeneric () = ix1b_NOT_IF_OPT <- ix1b_NOT_IF_OPT + 1
        localFunctionThatIsNotGeneric()

    let test1c() =
        let mutable ix1c_NOT_IF_OPT = 1
        let unusedLocalFunctionThatIsNotGeneric () = ix1c_NOT_IF_OPT <- ix1c_NOT_IF_OPT + 1
        ()

    let test1bg() =

        let mutable ix1bg_NOT_IF_OPT = 1
        let localFunctionThatIsGeneric (any: 'T) = ix1bg_NOT_IF_OPT <- ix1bg_NOT_IF_OPT + 1
        localFunctionThatIsGeneric()

    let test1cg() =
        let mutable ix1cg_NOT_IF_OPT = 1
        let unusedLocalFunctionThatIsGeneric (any: 'T) = ix1cg_NOT_IF_OPT <- ix1cg_NOT_IF_OPT + 1
        ()

    let test1d() =

        let mutable ix1d = 1
        let f () = 
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            ix1d <- ix1d + 1
        f()
        f()
        f()
        f()


    let test1e(ix1e_NO: byref<int>) =
        ix1e_NO <- ix1e_NO + 1



    let test1f() =
        let mutable ix1f_NOT_IF_OPT = 1
        let f () = 
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            printfn "don't inline me"
            ix1f_NOT_IF_OPT <- ix1f_NOT_IF_OPT + 1
        f() // immediate use of a function does get inlined

    (*
    [<Struct>]
    type S = 
       val mutable x : int
       member this_NO.M() = [ (fun () -> this_NO.x <- this_NO.x + 1) ]
*)

    let test2() =
        let mutable ix2 = 1
        [ (fun () -> 
                 let xaddr = &ix2
                 xaddr )]


    let test3() =
        let mutable ix3 = 1
        [ (fun () -> 
                 let xaddr = &ix3
                 xaddr <- 2 )]

    let test4() =
        let mutable ix4 = 1
        [ { new obj() with override __.ToString() = (ix4 <- ix4 + 1); "" } ]

    let test4b() =
        let ix4b_NO = ref 1
        [ { new obj() with override __.ToString() = (ix4b_NO := ix4b_NO.Value + 1); "" } ]


    let mutable ix5_NO = 1
    let test5() =
        [ (fun () -> ix5_NO <- ix5_NO + 1) ]


    let test6() =

        let mutable ix6_NO = 1
        let x = 3
        ix6_NO <- 3
        [ (fun () -> x) ]

    let test7() =
        let mutable ix7_NO = 1
        let x = 3
        let res = [ (fun () -> x) ]
        ix7_NO <- 3
        res

module StringtTests =

    let test1() =
        let mutable sx1 = "1"
        [ (fun () -> sx1 <- sx1 + "1")]

    let test2() =
        let mutable sx2 = "1"
        [ (fun () -> 
                 let xaddr = &sx2
                 xaddr )]


    let test3() =
        let mutable sx3 = "1"
        [ (fun () -> 
                 let xaddr = &sx3
                 xaddr <- "2" )]


module NestedLambdas =

    let test1() =

        let mutable nlx1 = 1
        [ (fun () -> [(fun () -> nlx1 <- nlx1 + 1)]) ]


    let test2() =
        [ (fun () -> 
             let mutable nlx2 = 1
             [(fun () -> nlx2 <- nlx2 + 1)]) ]


module SeqExpr =

    let test0() =
        seq { let mutable nlx1 = 1
              nlx1 <- nlx1 + 1
              yield nlx1
              yield nlx1 }


    let test0b() =
        seq { let nlx1 = ref 1
              yield nlx1
              yield nlx1 } // leaky escaping reference cells

    let test1() =
        seq { let mutable nlx1 = 1
              let f() = nlx1 <- nlx1 + 1
              yield f()
              yield f() }

    let test2() =
        seq { for x in [1..10] do
                  let mutable nlx1 = 1
                  let f() =  
                      nlx1 <- nlx1 + 1; 
                      nlx1
                  let g() = nlx1 
                  yield f
                  yield g }

    let test3() =
        let mutable seq3 = 1
        seq { yield (seq { yield seq3 }) }


module AsyncExpr =

    let test0() =
        async { let mutable nlx1 = 1
                nlx1 <- 3
                do! Async.Sleep 10
                printfn "nlx1 = %d" nlx1
                nlx1 <- nlx1 + 1 
                printfn "nlx1 = %d" nlx1
                do! Async.Sleep 10
                printfn "nlx1 = %d" nlx1
                }

    let _ = test0() |> Async.RunSynchronously



module TryWith =
    let foo o s = 
         let mutable x_NO = 1
         try 
            o s x_NO
         with exn ->
            x_NO <- 5

    let foo2 o s = 
         let mutable x_NO = 1
         try 
            o s x_NO
            x_NO <- 5
         with exn ->
            ()

    let bar o s = 
         let mutable x = 1
         try
            o ()
         with exn -> 
            s := (fun () -> x <- x + 1)

module TryFinally =
    let foo o s = 
         let mutable x_NO = 1
         try 
            o s x_NO
         finally
            x_NO <- 5

    let foo2 o s = 
         let mutable x_NO = 1
         try 
            o s x_NO
            x_NO <- 5
         finally
            s ()

    let bar o s = 
         let mutable x = 1
         try
            o ()
         finally
            s := (fun () -> x <- x + 1)


module ForLoop =
    let foo o s = 
         let mutable x_NO = 1
         for i in o do
            x_NO <- x_NO + 1

    let bar o s = 
         let mutable x = 1
         for i in o do
            s := (fun () -> x <- x + 1)

module WhileLoop =
    let foo o s = 
         let mutable x_NO = 1
         while (s x_NO) do 
            x_NO <- x_NO + 1

    let bar o s = 
         let mutable x = 1
         while o x do
            s := (fun () -> x <- x + 1)


module ExitModule =
    if ExecTests.check() then () else failwith "Failed"
