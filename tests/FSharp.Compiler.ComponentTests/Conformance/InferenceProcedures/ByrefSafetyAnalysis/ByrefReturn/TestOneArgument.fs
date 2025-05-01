open Prelude

module TestOneArgument =

    let f (x:byref<int>) = &x

    let test() = 
        let mutable r1 = 1
        let addr = &f &r1
        addr <- addr + 1
        check2 "cepojcwem10" 2 r1

    test()