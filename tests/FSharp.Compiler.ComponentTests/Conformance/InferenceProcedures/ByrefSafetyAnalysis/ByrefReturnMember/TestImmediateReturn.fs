open Prelude

module TestImmediateReturn =
    let mutable x = 1

    type C() = 
        static member M () = &x

    let test() = 
        let addr : byref<int> = &C.M()
        addr <- addr + 1
        check2 "mepojcwem1" 2 x


    let test2() = 
        let v = &C.M()
        let res = v + 1
        check2 "mepojcwem1b" 3 res

    test()
    test2()