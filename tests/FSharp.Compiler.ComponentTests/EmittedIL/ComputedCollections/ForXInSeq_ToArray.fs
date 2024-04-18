﻿let f1 (seq: int seq) = [|for x in seq -> x|]
let f2 f (seq: int seq) = [|for x in seq -> f x|]
let f3 f (seq: int seq) = [|for x in seq -> f (); x|]
let f4 f g (seq: int seq) = [|for x in seq -> f (); g(); x|]
let f5 (seq: int seq) = [|for x in seq do yield x|]
let f6 f (seq: int seq) = [|for x in seq do f (); yield x|]
let f7 f g (seq: int seq) = [|for x in seq do f (); g (); yield x|]

let f8 f g (seq: int seq) = [|let y = f () in let z = g () in for x in seq -> x + y + z|]
let f9 f g (seq: int seq) = [|let y = f () in g (); for x in seq -> x + y|]
let f10 f g (seq: int seq) = [|f (); g (); for x in seq -> x|]
let f11 f g (seq: int seq) = [|f (); let y = g () in for x in seq -> x + y|]
