module Test
let f () = let mutable a = 1 in  let ab = &a in (fun () -> [ab].Length) (* trap: byref captured by function closure (explicit lambda) *)
