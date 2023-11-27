open Prelude

module TestTwoArguments =

    let f (x:byref<int>, y:byref<int>) = &x

    let test() = 
        let mutable r1 = 1
        let mutable r2 = 0
        let addr = &f (&r1, &r2)
        addr <- addr + 1
        check2 "cepojcwem11" 2 r1

    test()