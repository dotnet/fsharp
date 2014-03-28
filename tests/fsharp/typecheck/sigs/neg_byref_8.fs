module Test
let xop4 (v:byref<int>) = let pair = &v,&v in [pair].Length  (* trap: tinst of TExpr_op, tuple - no opt away *)
