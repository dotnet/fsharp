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

    let test() = 
        let addr = &(C() :> I).M()
        addr <- addr + 1
        let addr = &(ObjExpr()).M()
        addr <- addr + 1
        check2 "mepojcwem16" 3 x

    test()