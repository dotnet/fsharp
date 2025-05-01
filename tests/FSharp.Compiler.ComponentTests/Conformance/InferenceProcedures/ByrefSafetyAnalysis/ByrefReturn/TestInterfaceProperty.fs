open Prelude

module TestInterfaceProperty =
    let mutable x = 1

    type I = 
        abstract P : byref<int>

    type C() = 
        interface I with 
            member this.P = &x

    let ObjExpr() = 
        { new I with 
            member this.P = &x }

    let f (i:I) = &i.P

    let test() = 
        let addr = &f (C()) 
        addr <- addr + 1
        let addr = &f (ObjExpr()) 
        addr <- addr + 1
        check2 "cepojcwem17" 3 x

    test()