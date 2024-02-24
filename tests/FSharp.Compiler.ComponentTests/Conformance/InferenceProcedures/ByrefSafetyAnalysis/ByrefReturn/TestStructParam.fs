open Prelude

module TestStructParam =

    [<Struct>]
    type R = { mutable z : int }

    let f (x:byref<R>) = &x.z

    let test() = 
        let mutable r = { z = 1 }
        let addr = &f &r
        addr <- addr + 1
        check2 "cepojcwem15" 2 r.z

    test()