(* Regression for Bug 3807 *)
module Test
let f (x:byref<int>) = "123"
let g = f (* <-- expect error here, but not ICE *)


