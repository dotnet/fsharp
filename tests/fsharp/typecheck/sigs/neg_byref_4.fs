module Test
type ID<'T> =
    static member id (x:'T) = x  
let vv  = ID<byref<int>> .id       (* trap: tinst of a generic type, then static method (TExpr_ilcall) *)
let f() = ID<byref<int>> .id       (* trap: tinst of a generic type, then static method (TExpr_ilcall) *)
