open Prelude

module TestDelegateMethod =
    let mutable x = 1

    type D = delegate of unit ->  byref<int>

    let test() = 
        let d = D(fun () -> &x)
        let addr = &d.Invoke()
        check2 "mepojcwem18a" 1 x
        addr <- addr + 1
        check2 "mepojcwem18b" 2 x

    test()