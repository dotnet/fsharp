open Prelude

module TestDelegateMethod =
    let mutable x = 1

    type D = delegate of unit ->  byref<int>

    let d() = D(fun () -> &x)

    let f (d:D) = &d.Invoke()

    let test() = 
        let addr = &f (d()) 
        check2 "cepojcwem18a" 1 x
        addr <- addr + 1
        check2 "cepojcwem18b" 2 x

    test()