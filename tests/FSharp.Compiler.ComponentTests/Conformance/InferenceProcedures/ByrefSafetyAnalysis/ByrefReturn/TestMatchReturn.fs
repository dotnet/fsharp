open Prelude

module TestMatchReturn =
    let mutable x = 1
    let mutable y = 1

    let f inp = match inp with 3 -> &x | _ -> &y

    let test() = 
        let addr = &f 3
        addr <- addr + 1
        check2 "cepojcwem2" 2 x
        check2 "cepojcwem3" 1 y
        let addr = &f 4
        addr <- addr + 1
        check2 "cepojcwem4" 2 x
        check2 "cepojcwem5" 2 y

    let test2() = 
        let res = f 3
        let res2 = res + 1
        check2 "cepojcwem2b" 3 res2
        check2 "cepojcwem3b" 2 res

    test()
    test2()