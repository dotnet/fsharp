open Prelude

module OutRefParam_Overloaded  = 
    type C() = 
         static member M(a: int, x: outref<int>) = x <- 7
         static member M(a: string, x: outref<int>) = x <- 8
    let mutable res = 9
    C.M("a", &res)
    check "cweweoiwek2v99323" res 8
    C.M(3, &res)
    check "cweweoiwe519" res 7