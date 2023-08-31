open Prelude

module TestOneArgument =

    type C() = 
        static member M (x:byref<int>) = &x

    let test() = 
        let mutable r1 = 1
        let addr = &C.M (&r1)
        addr <- addr + 1
        check2 "mepojcwem10" 2 r1

    test()