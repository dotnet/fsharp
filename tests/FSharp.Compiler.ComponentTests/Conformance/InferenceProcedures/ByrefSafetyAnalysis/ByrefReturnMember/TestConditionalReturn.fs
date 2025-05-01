open Prelude

module TestConditionalReturn =
    let mutable x = 1
    let mutable y = 1

    type C() = 
        static member M inp = if inp = 3 then &x else &y

    let test() = 
        let addr = &C.M 3
        addr <- addr + 1
        check2 "mepojcwem6" 2 x
        check2 "mepojcwem7" 1 y
        let addr = &C.M 4
        addr <- addr + 1
        check2 "mepojcwem8" 2 x
        check2 "mepojcwem9" 2 y

    let test2() = 
        let res = &C.M 3
        let res2 = res + 1
        check2 "mepojcwem8b" 3 res2
        check2 "mepojcwem9b" 2 res

    test()
    test2()