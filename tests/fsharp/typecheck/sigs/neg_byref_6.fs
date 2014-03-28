module Test
let xop2 = [| |] : byref<int>[]                              (* trap: tinst of TExpr_op, array *)
