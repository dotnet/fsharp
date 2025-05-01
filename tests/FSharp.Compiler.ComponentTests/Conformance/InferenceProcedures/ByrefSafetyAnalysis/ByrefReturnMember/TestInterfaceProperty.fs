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

    let test() = 
        let addr = &(C() :> I).P
        addr <- addr + 1
        let addr = &(ObjExpr()).P
        addr <- addr + 1
        check2 "mepojcwem17" 3 x

    test()