open Prelude

module TestStructParam =

    [<Struct>]
    type R = { mutable z : int }

    type C() = 
        static member M (x:byref<R>) = &x.z

    let test() = 
        let mutable r = { z = 1 }
        let addr = &C.M(&r)
        addr <- addr + 1
        check2 "mepojcwem15" 2 r.z

    test()