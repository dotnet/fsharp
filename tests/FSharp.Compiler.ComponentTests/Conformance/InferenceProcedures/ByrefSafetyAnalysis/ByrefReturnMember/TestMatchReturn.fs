open Prelude

module TestMatchReturn =
    let mutable x = 1
    let mutable y = 1

    type C() = 
        static member M inp = match inp with 3 -> &x | _ -> &y

    let test() = 
        let addr = &C.M 3
        addr <- addr + 1
        check2 "mepojcwem2" 2 x
        check2 "mepojcwem3" 1 y
        let addr = &C.M 4
        addr <- addr + 1
        check2 "mepojcwem4" 2 x
        check2 "mepojcwem5" 2 y

    let test2() = 
        let res = &C.M 3
        let res2 = res + 1
        check2 "mepojcwem2b" 3 res2
        check2 "mepojcwem3b" 2 res

    test()
    test2()
