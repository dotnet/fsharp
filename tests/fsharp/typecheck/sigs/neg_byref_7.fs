module Test
let xop3 (v:byref<int>) = let pair = &v,&v in [pair].Length  (* trap: tinst of TExpr_op, tuple - no opt away *)

let f2 (x: byref<int>) = let y = &x in &y
