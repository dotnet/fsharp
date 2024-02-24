open Prelude

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