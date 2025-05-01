open Prelude

module TestRecordParam =

    type R = { mutable z : int }
    let f (x:R) = &x.z

    let test() = 
        let r = { z = 1 }
        let addr = &f r
        addr <- addr + 1
        check2 "cepojcwem12" 2 r.z

    test()