open Prelude

module TestDelegateMethod2 =
    let mutable x = 1

    type D = delegate of byref<int> ->  byref<int>

    let d = D(fun xb -> &xb)

    let test() = 
        let addr = &d.Invoke(&x)
        check2 "mepojcwem18a2" 1 x
        addr <- addr + 1
        check2 "mepojcwem18b3" 2 x

    test()