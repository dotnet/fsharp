open Prelude

module TestClassParamMutableField =

    type C() = [<DefaultValue>] val mutable z : int

    type C2() = 
        static member M (x:C) = &x.z

    let test() = 
        let c = C()
        let addr = &C2.M c
        addr <- addr + 1
        check2 "mepojcwem13b" 1 c.z 

    test()