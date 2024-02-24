open Prelude

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