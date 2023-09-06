open Prelude

module OutRefParam  = 
    type C() = 
         static member M(x: outref<int>) = x <- 5
    let mutable res = 9
    C.M(&res)
    check "cweweoiwek28989" res 5