open Prelude

module TestTryWithReturn =
    let mutable x = 1
    let mutable y = 1

    let f inp = try &x with _ -> &y

    let test() = 
        let addr = &f 3
        addr <- addr + 1
        check2 "cepojcwem6b" 2 x
        check2 "cepojcwem7b" 1 y
        let addr = &f 4
        addr <- addr + 1
        check2 "cepojcwem8b" 3 x
        check2 "cepojcwem9b" 1 y

    let test2() = 
        let res = f 3
        let res2 = res + 1
        check2 "cepojcwem2ff" 4 res2
        check2 "cepojcwem3gg" 3 res

    test()
    test2()