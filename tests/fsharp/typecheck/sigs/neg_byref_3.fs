module Test
let vvv = List.length<byref<int>>  (* trap: tinst of a generic app (TExpr_app) *)
let f() = List.length<byref<int>>  (* trap: tinst of a generic app (TExpr_app) *)
