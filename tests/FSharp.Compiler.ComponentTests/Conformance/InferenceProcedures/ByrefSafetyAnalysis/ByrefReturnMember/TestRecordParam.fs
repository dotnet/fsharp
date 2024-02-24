open Prelude

module TestRecordParam =

    type R = { mutable z : int }
    type C() = 
        static member M (x:R) = &x.z

    let test() = 
        let r = { z = 1 }
        let addr = &C.M r
        addr <- addr + 1
        check2 "mepojcwem12" 2 r.z

    test()