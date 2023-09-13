open Prelude

module TestTryFinallyReturn =
    let mutable x = 1
    let mutable y = 1

    type C() = 
        static member M inp = try &x with _ -> &y

    let test() = 
        let addr = &C.M 3
        addr <- addr + 1
        check2 "mepojcwem6b" 2 x
        check2 "mepojcwem7b" 1 y
        let addr = &C.M 4
        addr <- addr + 1
        check2 "mepojcwem8b" 3 x
        check2 "mepojcwem9b" 1 y

    let test2() = 
        let res = &C.M 3
        let res2 = res + 1
        check2 "mepojcwem2tf" 4 res2
        check2 "mepojcwem3qw" 3 res

    test()
    test2()