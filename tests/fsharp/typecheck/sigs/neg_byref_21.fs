(* Regression for Bug 1358 *)
module Test
let f (x : byref<'a>) = x = x;;
let zz = f;;
let _ = (1 = 1.0) // deliberate error - the above passes now