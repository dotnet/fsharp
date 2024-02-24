open Prelude

module TestArrayParam =

    type C() = 
        static member M (x:int[]) = &x.[0]

    let test() = 
        let r = [| 1 |]
        let addr = &C.M r
        addr <- addr + 1
        check2 "mepojcwem14" 2 r.[0]

    test()