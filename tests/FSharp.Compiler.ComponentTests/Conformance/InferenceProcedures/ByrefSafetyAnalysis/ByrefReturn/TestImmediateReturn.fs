open Prelude

module TestImmediateReturn =
    let mutable x = 1

    let f () = &x

    let test() = 
        let addr : byref<int> = &f()
        addr <- addr + 1
        check2 "cepojcwem1" 2 x


    let test2() = 
        let v = f()
        let res = v + 1
        check2 "cepojcwem1b" 3 res

    test()
    test2()