open Prelude

module TestArrayParam =

    let f (x:int[]) = &x.[0]

    let test() = 
        let r = [| 1 |]
        let addr = &f r
        addr <- addr + 1
        check2 "cepojcwem14" 2 r.[0]

    test()