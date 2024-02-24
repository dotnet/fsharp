open Prelude

module ByRefParam_OverloadedTest_ExplicitOutAttribute  = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 7
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2cbe9" res 8
    C.M(3, &res)
    check "cweweoiwek28498" res 7