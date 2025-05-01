open Prelude

module InRefParam_ExplicitInAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<int>) = ()
    let mutable res = 9
    C.M(&res)
    check "cweweoiwe519btr" res 9