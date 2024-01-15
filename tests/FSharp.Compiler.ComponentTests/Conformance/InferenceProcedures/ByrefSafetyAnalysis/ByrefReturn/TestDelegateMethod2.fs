open Prelude

module TestDelegateMethod2 =
    let mutable x = 1

    type D = delegate of byref<int> ->  byref<int>

    let d() = D(fun xb -> &xb)

    let f (d:D) = &d.Invoke(&x)

    let test() = 
        let addr = &f (d()) 
        check2 "cepojcwem18a2" 1 x
        addr <- addr + 1
        check2 "cepojcwem18b3" 2 x

    test()