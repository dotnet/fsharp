open Prelude

module TestInterfaceMethod =
    let mutable x = 1

    type I = 
        abstract M : unit -> byref<int>

    type C() = 
        interface I with 
            member this.M() = &x

    let ObjExpr() = 
        { new I with 
            member this.M() = &x }

    let f (i:I) = &i.M()

    let test() = 
        let addr = &f (C()) 
        addr <- addr + 1
        let addr = &f (ObjExpr()) 
        addr <- addr + 1
        check2 "cepojcwem16" 3 x

    test()