open Prelude

module TestClassParamMutableField =

    type C() = [<DefaultValue>] val mutable z : int

    let f (x:C) = &x.z

    let test() = 
        let c = C()
        let addr = &f c
        addr <- addr + 1
        check2 "cepojcwem13b" 1 c.z 

    test()
