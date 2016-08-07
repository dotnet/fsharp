(* Regression for Bug 1358 *)
module Test
let rec f () = f () : int byref;;

let x = (1.0 = 1)

