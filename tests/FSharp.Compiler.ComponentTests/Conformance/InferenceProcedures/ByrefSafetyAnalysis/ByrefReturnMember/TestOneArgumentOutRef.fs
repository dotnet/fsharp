open Prelude

module TestOneArgumentOutRef =

    type C() = 
        static member M (x:outref<int>) = &x

    let test() = 
        let mutable r1 = 1
        let addr = &C.M (&r1)
        addr <- addr + 1
        check2 "mepojcwem10" 2 r1

    test()