type R1 = { B : int; C : int }
type R2 = { A : int; B : int; C : int }

let r1 = { B = 1; C = 2 }
let r2 = { A = 3; ...r1 }

let r1' = {| B = 1; C = 2 |}
let r2' = { A = 3; ...r1 }
