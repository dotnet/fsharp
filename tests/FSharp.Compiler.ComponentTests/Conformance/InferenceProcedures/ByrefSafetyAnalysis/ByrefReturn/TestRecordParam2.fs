open Prelude

module TestRecordParam2 =

    type R = { mutable z : int }
    let f (x:byref<R>) = &x.z

    let test() = 
        let mutable r = { z = 1 }
        let addr = &f &r
        addr <- addr + 1
        check2 "cepojcwem13a" 2 r.z

    test()