open Prelude

module TestConditionalReturn =
    let mutable x = 1
    let mutable y = 1

    let f inp = if inp = 3 then &x else &y

    let test() = 
        let addr = &f 3
        addr <- addr + 1
        check2 "cepojcwem6" 2 x
        check2 "cepojcwem7" 1 y
        let addr = &f 4
        addr <- addr + 1
        check2 "cepojcwem8" 2 x
        check2 "cepojcwem9" 2 y

    let test2() = 
        let res = f 3
        let res2 = res + 1
        check2 "cepojcwem8b" 3 res2
        check2 "cepojcwem9b" 2 res

    test()
    test2()