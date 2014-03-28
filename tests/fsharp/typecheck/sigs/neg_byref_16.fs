module Test
let rec pair x y loop = if loop then pair x y loop else (x,y)
let f () = let mutable a = 1 in pair (&a)                  (* trap: byref captured by function closure (partial application) *)
