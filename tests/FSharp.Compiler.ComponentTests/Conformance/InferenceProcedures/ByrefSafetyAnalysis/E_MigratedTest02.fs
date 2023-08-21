// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_byrefs
#endif

module NegativeTests =

    let test1 doIt =
        let mutable x = 42
        let r =
            if doIt then
                let mutable y = 1
                &y // not allowed
            else
                &x

        let c = 
            if doIt then
                let mutable z = 2
                &z // not allowed
            else
                &x

        x + r + c

    let test2 () =
        let x =
            let mutable x = 1
            &x // not allowed

        let y =
            let mutable y = 2
            &y // not allowed

        x + y

    let test3 doIt =
        let mutable x = 1
        if doIt then
            &x // not allowed
        else
            let mutable y = 1
            &y // not allowed

    let test4 doIt =
        let mutable x = 1
        let y =
            if doIt then
                &x
            else
                let mutable z = 1
                &z // not allowed
        &y // not allowed

    type Coolio() =

        static member Cool(x: inref<int>) = &x

    let test5 () =

        let y =
            let x = 1
            &Coolio.Cool(&x) // not allowed

        () 

    let test6 () =

        let y =
            let mutable x = 1
            &Coolio.Cool(&x) // not allowed

        () 

    let test7 () =
        let mutable x = 1
        let f = fun () -> &x // not allowed
        
        ()
        
    type ByRefInterface =

        abstract Test : byref<int> * byref<int> -> byref<int>

    type Test() =

        member __.TestMethod() =
            let mutable a = Unchecked.defaultof<ByRefInterface>
            let obj = { new ByRefInterface with

                member __.Test(x,y) =
                    let mutable x = 1
                    let obj2 =
                        { new ByRefInterface with

                            member __.Test(_x,y) = &x } // is not allowed
                    a <- obj2
                    &y
            }
            let mutable x = 500
            let mutable y = 500
            obj.Test(&x, &y) |> ignore
            a
            
    type TestDelegate = delegate of unit-> byref<int>
    let testFunction () =
        let mutable x = 1
        let f = TestDelegate(fun () -> &x) // is not allowed
        ()

    type TestNegativeOverloading() =

        static member TestMethod(dt: byref<int>) = ()

        static member TestMethod(dt: inref<int>) = ()

        static member TestMethod(dt: outref<int>) = ()

    type NegativeInterface =

        abstract Test : (byref<int> * byref<int>) -> byref<int>

        abstract Test2 : (byref<int> -> unit) -> unit

    let test8 (x: byref<int>) = (&x, 1)

    let test9 (x: byref<int>) =
        printfn "test9"
        fun (y: byref<int>) -> ()

    let test10 (x: (byref<int> -> unit) * int) = ()

    let test11 (x: byref<int> -> unit) (y: byref<int> * int) = ()

    type StaticTest private () =

        static member Test (x: inref<int>, y: int) = ()

        static member NegativeTest(tup) =
            StaticTest.Test(tup)

        static member NegativeTest2(x: byref<int> -> unit) = ()

        static member NegativeTest3(x: byref<int> option) = ()

    let test12 (x: byref<int> option) = ()

    let testHelper1 (x: int) (y: byref<int>) = ()

    let test13 () =
        let x = testHelper1 1
        ()

    let test14 () =
        testHelper1 1

    let test15 () =
        let x =
            printfn "test"
            testHelper1 1
        ()

    let test16 () =
        let x = 1
        testHelper1 1
        ()

    let test17 () =
        let x = testHelper1
        ()

    let test18 () =
        testHelper1

    let test19 () =
        let x =
            printfn "test"
            testHelper1
        ()

    let test20 () =
        let x = 1
        testHelper1
        ()

    let test21 () =
        let x = StaticTest.Test
        ()

    let test22 () =
        let x =
            printfn "test"
            StaticTest.Test
        ()

    let test23 () =
        let x = 1
        StaticTest.Test
        ()

    let test24 () : byref<int> * int =
        let mutable x = 1
        (&x, 1)

    let test25 () =
        test24 ()
        ()

    let test26 () =
        let x = test24 ()
        ()
    