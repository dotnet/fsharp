(* Regression for Bug 1358 *)
module Test
let f (x : byref<'a>) = x = x;;
let zz = f;;
