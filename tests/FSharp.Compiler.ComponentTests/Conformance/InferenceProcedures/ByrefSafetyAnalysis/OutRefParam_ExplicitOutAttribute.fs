open Prelude

module OutRefParam_ExplicitOutAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 5
    let mutable res = 9
    C.M(&res)
    check "cweweoiweklceew4" res 5