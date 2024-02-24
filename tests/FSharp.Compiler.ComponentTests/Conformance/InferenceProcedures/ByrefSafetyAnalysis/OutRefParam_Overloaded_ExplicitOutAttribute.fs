open Prelude

module OutRefParam_Overloaded_ExplicitOutAttribute   = 
    type C() = 
         static member M(a: int, [<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 7
         static member M(a: string, [<System.Runtime.InteropServices.Out>] x: outref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2v90" res 8
    C.M(3, &res)
    check "cweweoiwek2c98" res 7